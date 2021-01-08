using SessionManagerExtension.Utils;
using SessionManagerExtension.Windows.ToolWindows;
using System;
using System.Threading.Tasks;
using VsShell = Microsoft.VisualStudio.Shell;

namespace SessionManagerExtension.Commands
{
    internal sealed class SessionManagerToolWindowCommand : ISessionManagerCommand
    {
        private readonly SessionManagerPackage _package;

        public SessionManagerToolWindowCommand(PackageAccessor packageAccessor)
        {
            _package = packageAccessor.Package;
        }

        public Guid CommandSet => new Guid("e5e58af2-0280-4a35-89ff-547922fb526f");
        public int CommandId => 0x0100;


        public async Task ExecuteAsync()
        {
            VsShell.ToolWindowPane window = await _package.ShowToolWindowAsync(typeof(SessionManagerToolWindow), 0, true, _package.DisposalToken);
            if ((null == window) || (null == window.Frame))
            {
                throw new NotSupportedException("Cannot create tool window");
            }
        }

        public void OnBeforeQueryCommandStatus(VsShell.OleMenuCommand command)
        {
        }
    }
}
