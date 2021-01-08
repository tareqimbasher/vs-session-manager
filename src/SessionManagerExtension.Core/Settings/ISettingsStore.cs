using System.Threading.Tasks;

namespace SessionManagerExtension.Settings
{
    public interface ISettingsStore
    {
        Task<SolutionSettings> GetSolutionSettingsAsync(string solutionFilePath);
        Task SaveSolutionSettingsAsync(string solutionFilePath, SolutionSettings settings);
    }
}
