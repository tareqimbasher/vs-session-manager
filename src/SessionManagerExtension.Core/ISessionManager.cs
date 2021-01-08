using SessionManagerExtension.Settings;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SessionManagerExtension
{
    public interface ISessionManager
    {
        bool HasSolutionSettingsLoaded { get; }
        SolutionSettings SolutionSettings { get; }

        Task LoadSolutionSettingsAsync(string solutionFilePath);
        Task<Session> AddSessionAsync(string name);
        Task<Session> AddSessionAsync(string name, IEnumerable<Document> documents, string documentPositions);
        Task RenameSessionAsync(Session session, string newName);
        Task DeleteSessionAsync(Session session);
        Task SaveSolutionSettingsAsync();
    }
}
