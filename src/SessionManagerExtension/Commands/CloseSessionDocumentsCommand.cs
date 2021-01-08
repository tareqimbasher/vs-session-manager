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
    public class CloseSessionDocumentsCommand : ISessionManagerCommand
    {
        private readonly IIDE _ide;
        private readonly SessionManagerToolWindowState _sessionManagerToolWindowState;
        private readonly SessionManagerPackage _package;

        public Guid CommandSet => SessionManagerToolWindowCommandSet.Guid;
        public int CommandId => SessionManagerToolWindowCommandSet.CloseSessionDocumentsId;

        public CloseSessionDocumentsCommand(IIDE ide, SessionManagerToolWindowState sessionManagerToolWindowState, PackageAccessor packageAccessor)
        {
            _ide = ide;
            _sessionManagerToolWindowState = sessionManagerToolWindowState;
            _package = packageAccessor.Package;
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

            var sessionDocumentPaths = session.Documents.Select(d => d.FullPath.ToLowerInvariant()).ToHashSet();
            await _package.JoinableTaskFactory.SwitchToMainThreadAsync();

            foreach (var document in _ide.Documents)
            {
                if (document.FullName == null)
                    continue;
                if (sessionDocumentPaths.Contains(document.FullName.ToLowerInvariant()))
                    document.Close();
            }
        }

        public void OnBeforeQueryCommandStatus(VsShell.OleMenuCommand command)
        {
            command.Enabled = _sessionManagerToolWindowState.SelectedSession != null;
        }
    }
}
