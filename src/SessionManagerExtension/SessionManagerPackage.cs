using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using SessionManagerExtension.Commands;
using SessionManagerExtension.DependencyInjection;
using SessionManagerExtension.Settings;
using SessionManagerExtension.Utils;
using SessionManagerExtension.Windows.ToolWindows;
using Task = System.Threading.Tasks.Task;

namespace SessionManagerExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
    [Guid(SessionManagerPackage.PackageGuidString)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [ProvideToolWindow(typeof(SessionManagerToolWindow))]
    public sealed class SessionManagerPackage : AsyncPackage
    {
        /// <summary>
        /// SessionManagerPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "b530e711-8043-49b5-b82d-7b94565cfc87";
        private static Container _container;
        private static Container.IScope _serviceProvider;
        private static SolutionEvents _solutionEvents;

        public SessionManagerPackage()
        {
        }

        public static IServiceProvider ServiceProvider => _serviceProvider;


        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token to monitor for initialization cancellation, which can occur when VS is shutting down.</param>
        /// <param name="progress">A provider for progress updates.</param>
        /// <returns>A task representing the async work of package initialization, or an already completed task if there is none. Do not return null from this method.</returns>
        protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            // When initialized asynchronously, the current thread may be a background thread at this point.
            // Do any initialization that requires the UI thread after switching to the UI thread.
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
            UiThread.Capture();

            await ConfigureServicesAsync();

            if (cancellationToken.IsCancellationRequested)
                return;

            progress.Report(new ServiceProgressData("Initializing Session Manager"));

            await BindEventsAsync(cancellationToken);

            // Initialize commands
            var menuCommandService = _serviceProvider.Resolve<IMenuCommandService>();
            foreach (var command in _serviceProvider.Resolve<IEnumerable<ISessionManagerCommand>>())
            {
                var commandId = new CommandID(command.CommandSet, command.CommandId);
                var menuCommand = new OleMenuCommand((s, e) => JoinableTaskFactory.RunAsync(async () => await command.ExecuteAsync()), commandId);
                menuCommand.BeforeQueryStatus += (s, e) =>
                {
                    if (s is OleMenuCommand oleMenuCommand)
                        command.OnBeforeQueryCommandStatus(oleMenuCommand);
                };
                menuCommandService.AddCommand(menuCommand);
            }
        }

        private async Task ConfigureServicesAsync()
        {
            var environment = await GetServiceAsync(typeof(DTE)) as DTE2;
            var documentWindowMgr = await GetServiceAsync(typeof(IVsUIShellDocumentWindowMgr)) as IVsUIShellDocumentWindowMgr;
            var commandService = await GetServiceAsync(typeof(IMenuCommandService)) as OleMenuCommandService;

            _container = new Container();
            _container.Register<PackageAccessor>(() => new PackageAccessor(this)).AsSingleton();
            _container.Register<IMenuCommandService>(() => commandService).AsSingleton();
            _container.Register<DTE2>(() => environment).AsSingleton();
            _container.Register<IVsUIShellDocumentWindowMgr>(() => documentWindowMgr).AsSingleton();
            _container.Register<IIDE, IDE>().AsSingleton();

            _container.Register<ISessionManager, SessionManager>().AsSingleton();
            _container.Register<IJsonSerializer, JsonSerializer>().AsSingleton();
            _container.Register<ISettingsStore, FileSettingsStore>().AsSingleton();
            _container.Register<ILogger, DefaultLogger>().AsSingleton();

            _container.Register<SessionManagerToolWindow>().AsSingleton();
            _container.Register<SessionManagerToolWindowControl>().AsSingleton();
            _container.Register<SessionManagerToolWindowState>().AsSingleton();

            // Commands
            var commands = new[]
            {
                typeof(SessionManagerToolWindowCommand),
                typeof(SaveCurrentSessionCommand),
                typeof(OpenSessionCommand),
                typeof(RestoreSessionCommand),
                typeof(CloseSessionDocumentsCommand),
                typeof(DeleteSessionCommand),
            };
            
            // Register each command
            foreach (var commandType in commands)
            {
                _container.Register(commandType, commandType).AsSingleton();
            }

            // Register collection of commands
            _container.Register<IEnumerable<ISessionManagerCommand>>(() =>
            {
                return commands.Select(c => _serviceProvider.GetService(c) as ISessionManagerCommand).ToArray();
            });

            //_container.Register<SessionManagerToolWindowCommand>().AsSingleton();

            _serviceProvider = _container.CreateScope();
            _container.Register<IServiceProvider>(() => _serviceProvider).AsSingleton();
        }

        private async Task BindEventsAsync(CancellationToken cancellationToken)
        {
            await this.JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);

            var ide = _serviceProvider.Resolve<IIDE>();

            // Capture solution events. Otherwise it gets GC'ed.
            _solutionEvents = ide.Environment.Events.SolutionEvents;

            _solutionEvents.Opened += OnSolutionOpensOrCloses;
            _solutionEvents.AfterClosing += OnSolutionOpensOrCloses;

            // This package could load after a solution has loaded. If a solution is loaded and the session manager has 
            // not yet loaded any solution settings, fire the event.
            var sessionManager = _serviceProvider.Resolve<ISessionManager>();
            if (sessionManager.HasSolutionSettingsLoaded == false && ide.Environment.Solution?.IsOpen == true)
            {
                OnSolutionOpensOrCloses();
            }
        }

        private void OnSolutionOpensOrCloses()
        {
            JoinableTaskFactory.RunAsync(async () =>
            {
                await JoinableTaskFactory.SwitchToMainThreadAsync();
                var ide = _serviceProvider.Resolve<IIDE>();
                var sessionManager = _serviceProvider.Resolve<ISessionManager>();
                await sessionManager.LoadSolutionSettingsAsync(ide.Environment.Solution?.FullName);
            });
        }

        protected override WindowPane InstantiateToolWindow(Type toolWindowType)
        {
            return (WindowPane)_serviceProvider.GetService(toolWindowType);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _solutionEvents.Opened -= OnSolutionOpensOrCloses;
                _solutionEvents.AfterClosing -= OnSolutionOpensOrCloses;
                _container?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
