using System.Threading.Tasks;

namespace SessionManagerExtension.Settings
{
    /// <summary>
    /// Provides a mechanism to store and retreive settings.
    /// </summary>
    public interface ISettingsStore
    {
        Task<SolutionSettings> GetSolutionSettingsAsync(string solutionFilePath);
        Task SaveSolutionSettingsAsync(string solutionFilePath, SolutionSettings settings);
    }
}
