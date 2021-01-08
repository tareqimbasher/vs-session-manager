using SessionManagerExtension.Dialogs;
using SessionManagerExtension.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VsShell = Microsoft.VisualStudio.Shell;

namespace SessionManagerExtension.Commands
{
    public class SaveCurrentSessionCommand : ISessionManagerCommand
    {
        private readonly SessionManagerPackage _package;
        private readonly IIDE _ide;
        private readonly ISessionManager _sessionManager;
        private readonly ILogger _logger;

        private static object _saveLock;
        private static bool _isSavingSession = false;

        public SaveCurrentSessionCommand(
            PackageAccessor packageAccessor,
            ISessionManager sessionManager,
            IIDE ide,
            ILogger logger)
        {
            _package = packageAccessor.Package;
            _sessionManager = sessionManager;
            _ide = ide;
            _logger = logger;
            _saveLock = new object();
        }

        public Guid CommandSet => SessionManagerToolWindowCommandSet.Guid;
        public int CommandId => SessionManagerToolWindowCommandSet.SaveCurrentSessionId;

        public async Task ExecuteAsync()
        {
            await ExecuteAsync(null);
        }

        public async Task ExecuteAsync(Session session)
        {
            if (_isSavingSession)
            {
                return;
            }

            lock (_saveLock)
            {
                _isSavingSession = true;
            }

            try
            {
                await _package.JoinableTaskFactory.SwitchToMainThreadAsync();
                var openDocuments = _ide.Documents.ToArray();

                if (!openDocuments.Any())
                {
                    return;
                }

                var documentPositions = await _ide.GetDocumentWindowPositionsAsync();
                if (documentPositions == null)
                {
                    _logger.ErrorAndShowPopupMessage("Could not save new session. Failed to get document window positions.");
                    return;
                }

                var documents = new List<Document>();
                foreach (var document in openDocuments)
                {
                    bool isProjectItem = !string.IsNullOrWhiteSpace(document.ProjectItem?.ContainingProject?.FullName);
                    documents.Add(new Document(document.Name, document.FullName, isProjectItem));
                }

                if (session == null)
                {
                    // Ask user for new name of session
                    var dialog = new SessionNameDialog();
                    if (dialog.ShowDialog() != true)
                    {
                        return;
                    }

                    await _sessionManager.AddSessionAsync(
                        dialog.SessionName,
                        documents,
                        documentPositions
                    );
                }
                else
                {
                    session.Documents.Clear();
                    foreach (var document in documents)
                    {
                        session.AddDocument(document.Name, document.FullPath, document.IsProjectItem);
                    }
                    session.DocumentPositions = documentPositions;
                    await _sessionManager.SaveSolutionSettingsAsync();
                }
                
            }
            catch (Exception ex)
            {
                _logger.ErrorAndShowPopupMessage($"An error occurred while saving the current session.", ex);
            }
            finally
            {
                lock (_saveLock)
                {
                    _isSavingSession = false;
                }
            }
        }

        public void OnBeforeQueryCommandStatus(VsShell.OleMenuCommand command)
        {
        }
    }
}
