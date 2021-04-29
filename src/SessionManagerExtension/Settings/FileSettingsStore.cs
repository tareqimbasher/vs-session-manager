using SessionManagerExtension.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SessionManagerExtension.Settings
{
    /// <summary>
    /// Retreives settings from JSON configuration files.
    /// </summary>
    public class FileSettingsStore : ISettingsStore
    {
        private const string _solutionSettingsFileName = ".sessionmanager.json";
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ILogger _logger;

        public FileSettingsStore(IJsonSerializer jsonSerializer, ILogger logger)
        {
            _jsonSerializer = jsonSerializer;
            _logger = logger;
        }

        public Task<SolutionSettings> GetSolutionSettingsAsync(string solutionFilePath)
        {
            SolutionSettings settings;

            try
            {
                var settingsFilePath = GetSettingsFilePath(solutionFilePath);
                var settingsFileDir = Path.GetDirectoryName(settingsFilePath);

                if (!Directory.Exists(settingsFileDir) || !File.Exists(settingsFilePath))
                {
                    settings = SolutionSettings.Empty;
                }
                else
                {
                    settings = _jsonSerializer.Deserialize<SolutionSettings>(File.ReadAllText(settingsFilePath));
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"Error getting Session Manager settings for solution: \"{solutionFilePath}\". Details: {ex}");
                settings = SolutionSettings.Empty;
            }

            return Task.FromResult(settings);
        }

        public Task SaveSolutionSettingsAsync(string solutionFilePath, SolutionSettings settings)
        {
            try
            {
                var settingsFilePath = GetSettingsFilePath(solutionFilePath);
                var settingsFileDir = Path.GetDirectoryName(settingsFilePath);

                if (Directory.Exists(settingsFileDir))
                {
                    File.WriteAllText(settingsFilePath, _jsonSerializer.Serialize(settings));
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorAndShowPopupMessage($"Error saving Session Manager settings for solution: \"{solutionFilePath}\".", ex);
            }

            return Task.CompletedTask;
        }

        private string GetSettingsFilePath(string solutionFilePath)
        {
            return Path.Combine(Path.GetDirectoryName(solutionFilePath), _solutionSettingsFileName);
        }
    }
}
