using GongSolutions.Wpf.DragDrop;
using SessionManagerExtension.Commands;
using SessionManagerExtension.Dialogs;
using SessionManagerExtension.Utils;
using SessionManagerExtension.WPF;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SessionManagerExtension.Windows.ToolWindows
{
    /// <summary>
    /// Interaction logic for SessionManagerToolWindowControl.
    /// </summary>
    public partial class SessionManagerToolWindowControl : UserControl, IDropTarget
    {
        private readonly OpenSessionCommand _openSessionCommand;
        private readonly SessionManagerPackage _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="SessionManagerToolWindowControl"/> class.
        /// </summary>
        public SessionManagerToolWindowControl(
            SessionManagerToolWindowState state,
            ISessionManager sessionManager,
            SaveCurrentSessionCommand saveCurrentSessionCommand,
            RestoreSessionCommand restoreSessionCommand,
            OpenSessionCommand openSessionCommand,
            CloseSessionDocumentsCommand closeSessionDocumentsCommand,
            DeleteSessionCommand deleteSessionCommand,
            PackageAccessor packageAccessor)
        {
            State = state;
            SessionManager = sessionManager;
            _openSessionCommand = openSessionCommand;
            _package = packageAccessor.Package;

            RestoreSessionCommand = new RelayCommand(
                param => _package.JoinableTaskFactory.RunAsync(async () => await restoreSessionCommand.ExecuteAsync(State.SelectedSession)));
            OpenSessionCommand = new RelayCommand(
                param => _package.JoinableTaskFactory.RunAsync(async () => await openSessionCommand.ExecuteAsync(State.SelectedSession)));
            CloseSessionDocumentsCommand = new RelayCommand(
                param => _package.JoinableTaskFactory.RunAsync(async () => await closeSessionDocumentsCommand.ExecuteAsync(State.SelectedSession)));
            DeleteSessionCommand = new RelayCommand(
                param => _package.JoinableTaskFactory.RunAsync(async () => await deleteSessionCommand.ExecuteAsync(State.SelectedSession)));
            RenameSessionCommand = new RelayCommand(
                param => _package.JoinableTaskFactory.RunAsync(async () => await RenameSessionAsync()));
            UpdateSessionCommand = new RelayCommand(
                param => _package.JoinableTaskFactory.RunAsync(async () =>
                {
                    if (MessageBox.Show($"Documents saved in \"{State.SelectedSession.Name}\" will be replaced with " +
                        $"the currently opened documents. Are you sure you want to continue?",
                        "Confirm",
                        MessageBoxButton.YesNo,
                        MessageBoxImage.Question) != MessageBoxResult.Yes)
                        return;
                    await saveCurrentSessionCommand.ExecuteAsync(State.SelectedSession);
                }));

            this.InitializeComponent();
        }

        public SessionManagerToolWindowState State { get; }
        public ISessionManager SessionManager { get; }

        public ICommand RestoreSessionCommand { get; set; }
        public ICommand OpenSessionCommand { get; set; }
        public ICommand CloseSessionDocumentsCommand { get; set; }
        public ICommand DeleteSessionCommand { get; set; }
        public ICommand RenameSessionCommand { get; set; }
        public ICommand UpdateSessionCommand { get; set; }

        public async Task RenameSessionAsync()
        {
            Session session = State.SelectedSession;
            if (session == null)
            {
                return;
            }

            string currentName = session.Name;

            // Ask user for new name of session
            var dialog = new SessionNameDialog(currentName);
            if (dialog.ShowDialog() != true || dialog.SessionName == currentName)
            {
                return;
            }

            await SessionManager.RenameSessionAsync(session, dialog.SessionName);
        }


        private void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            if (gView.Columns.Count == 1)
            {
                gView.Columns[0].Width = workingWidth;
            }
            if (gView.Columns.Count == 2)
            {
                var col1 = 0.30;
                var col2 = 0.70;

                gView.Columns[0].Width = workingWidth * col1;
                gView.Columns[1].Width = workingWidth * col2;
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (State.SelectedSession == null)
            {
                return;
            }

            _package.JoinableTaskFactory.RunAsync(async () => await _openSessionCommand.ExecuteAsync(State.SelectedSession));
        }

        void IDropTarget.DragOver(IDropInfo dropInfo)
        {
            dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
            dropInfo.Effects = DragDropEffects.Copy;
        }

        void IDropTarget.Drop(IDropInfo dropInfo)
        {
            if (!(dropInfo.Data is Session draggedSession))
                return;

            var sessions = SessionManager.SolutionSettings.Sessions;

            int iDragIndex = dropInfo.DragInfo.SourceIndex;
            int iDropIndex = dropInfo.InsertIndex;

            if (iDragIndex == iDropIndex)
            {
                return;
            }

            sessions.Remove(draggedSession);

            if (iDropIndex == 0)
            {
                sessions.Insert(0, draggedSession);
            }
            else if (iDropIndex > sessions.Count - 1)
            {
                sessions.Add(draggedSession);
            }
            else
            {
                if (iDropIndex < iDragIndex)
                {
                    sessions.Insert(iDropIndex, draggedSession);
                }
                else
                {
                    sessions.Insert(iDropIndex - 1, draggedSession);
                }
            }

            _package.JoinableTaskFactory.RunAsync(async () => await SessionManager.SaveSolutionSettingsAsync());
        }
    }
}