using VsShell = Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SessionManagerExtension.Windows.ToolWindows;
using SessionManagerExtension.Utils;
using System.IO;

namespace SessionManagerExtension.Commands
{
    public class RestoreSessionCommand : ISessionManagerCommand
    {
        private readonly IIDE _ide;
        private readonly SessionManagerToolWindowState _sessionManagerToolWindowState;

        public Guid CommandSet => SessionManagerToolWindowCommandSet.Guid;
        public int CommandId => SessionManagerToolWindowCommandSet.RestoreSessionId;

        public RestoreSessionCommand(IIDE ide, SessionManagerToolWindowState sessionManagerToolWindowState)
        {
            _ide = ide;
            _sessionManagerToolWindowState = sessionManagerToolWindowState;
        }

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

            await _ide.RestoreDocumentWindowsAsync(session.DocumentPositions);

            foreach (var document in session.Documents.Where(d => d.IsProjectItem == false))
            {
                await _ide.OpenFileAsync(document.FullPath);
            }
        }

        public void OnBeforeQueryCommandStatus(VsShell.OleMenuCommand command)
        {
            command.Enabled = _sessionManagerToolWindowState.SelectedSession != null;
        }
    }
}
