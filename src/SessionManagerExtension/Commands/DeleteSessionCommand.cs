using SessionManagerExtension.Utils;
using SessionManagerExtension.Windows.ToolWindows;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using VsShell = Microsoft.VisualStudio.Shell;

namespace SessionManagerExtension.Commands
{
    public class DeleteSessionCommand : ISessionManagerCommand
    {
        private readonly ISessionManager _sessionManager;
        private readonly SessionManagerToolWindowState _sessionManagerToolWindowState;

        public DeleteSessionCommand(
            ISessionManager sessionManager,
            SessionManagerToolWindowState sessionManagerToolWindowState)
        {
            _sessionManager = sessionManager;
            _sessionManagerToolWindowState = sessionManagerToolWindowState;
        }

        public Guid CommandSet => SessionManagerToolWindowCommandSet.Guid;
        public int CommandId => SessionManagerToolWindowCommandSet.DeleteSessionId;

        public async Task ExecuteAsync()
        {
            if (_sessionManagerToolWindowState.SelectedSession != null)
            {
                await ExecuteAsync(_sessionManagerToolWindowState.SelectedSession);
            }
        }

        public async Task ExecuteAsync(Session session)
        {
            if (session == null)
            {
                return;
            }

            if (MessageBox.Show($"Are you sure you want to delete session \"{session.Name}\"?",
                "Confirm",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question) != MessageBoxResult.Yes)
            {
                return;
            }

            bool sessionToDeleteIsSelectedSession = session == _sessionManagerToolWindowState.SelectedSession;
            var sessionToDeleteIndex = _sessionManager.SolutionSettings.Sessions.IndexOf(session);

            await _sessionManager.DeleteSessionAsync(session);

            if (sessionToDeleteIsSelectedSession)
            {
                if (sessionToDeleteIndex <= 0)
                {
                    _sessionManagerToolWindowState.SelectedSession = _sessionManager.SolutionSettings.Sessions.FirstOrDefault();
                }
                else if (_sessionManager.SolutionSettings.Sessions.Count - 1 >= sessionToDeleteIndex)
                {
                    _sessionManagerToolWindowState.SelectedSession = _sessionManager.SolutionSettings.Sessions[sessionToDeleteIndex];
                }
                else
                {
                    _sessionManagerToolWindowState.SelectedSession = _sessionManager.SolutionSettings.Sessions.Last();
                }
            }
        }

        public void OnBeforeQueryCommandStatus(VsShell.OleMenuCommand command)
        {
            command.Enabled = _sessionManagerToolWindowState.SelectedSession != null;
        }
    }
}
