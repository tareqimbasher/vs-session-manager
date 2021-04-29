using SessionManagerExtension.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SessionManagerExtension
{
    public class SessionManager : NotifiesPropertyChanged, ISessionManager
    {
        private readonly ISettingsStore _settingsStore;
        private string? _solutionFilePath;
        private SolutionSettings _solutionSettings;

        public SessionManager(ISettingsStore settingsStore)
        {
            _settingsStore = settingsStore;
            _solutionSettings = SolutionSettings.Empty;
        }

        public bool HasSolutionSettingsLoaded { get; private set; }
        public SolutionSettings SolutionSettings
        {
            get => _solutionSettings;
            set => SetField(ref _solutionSettings, value);
        }

        public async Task LoadSolutionSettingsAsync(string solutionFilePath)
        {
            _solutionFilePath = solutionFilePath;

            if (string.IsNullOrWhiteSpace(solutionFilePath))
            {
                HasSolutionSettingsLoaded = false;
                SolutionSettings = SolutionSettings.Empty;
            }
            else
            {
                SolutionSettings = await _settingsStore.GetSolutionSettingsAsync(solutionFilePath);
            }
        }

        public async Task<Session> AddSessionAsync(string name)
        {
            var session = new Session(name);
            SolutionSettings.Sessions.Add(session);
            await SaveSolutionSettingsAsync(SolutionSettings);
            return session;
        }

        public async Task<Session> AddSessionAsync(string name, IEnumerable<Document> documents, string documentPositions)
        {
            var session = new Session(name);
            session.DocumentPositions = documentPositions;

            foreach (var document in documents)
            {
                if (document.Name == null || document.FullPath == null)
                    throw new ArgumentException("Document name and fullpath cannot be null.");

                session.AddDocument(document.Name, document.FullPath, document.IsProjectItem);
            }

            SolutionSettings.Sessions.Add(session);
            await SaveSolutionSettingsAsync(SolutionSettings);
            return session;
        }

        public async Task RenameSessionAsync(Session session, string newName)
        {
            if (!SolutionSettings.Sessions.Contains(session))
                throw new ArgumentException("This session is not part of this solution's settings.");

            if (session.Name == newName)
            {
                return;
            }

            session.Name = newName;
            await SaveSolutionSettingsAsync(SolutionSettings);
        }

        public async Task DeleteSessionAsync(Session session)
        {
            if (SolutionSettings.Sessions.Contains(session) == false)
            {
                return;
            }

            SolutionSettings.Sessions.Remove(session);
            await SaveSolutionSettingsAsync(SolutionSettings);
        }

        public async Task SaveSolutionSettingsAsync()
        {
            await SaveSolutionSettingsAsync(SolutionSettings);
        }

        private async Task SaveSolutionSettingsAsync(SolutionSettings settings)
        {
            if (_solutionFilePath == null)
            {
                return;
            }
            else
            {
                await _settingsStore.SaveSolutionSettingsAsync(_solutionFilePath, settings);
            }
        }
    }
}
