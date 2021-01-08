using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;
using SessionManagerExtension.Utils;
using System.Threading.Tasks;

namespace SessionManagerExtension.Settings
{
    public class VsSettingsStore
    {
        public const string SettingsCollectionName = "SessionManager";
        public const string AppSettingsPropertyName = "Settings";
        private readonly IJsonSerializer _jsonSerializer;

        public VsSettingsStore(IJsonSerializer jsonSerializer)
        {
            _jsonSerializer = jsonSerializer;
        }

        private async Task<SettingsStore> GetReadOnlySettingsStoreAsync()
        {
            await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
            return new ShellSettingsManager(ServiceProvider.GlobalProvider).GetReadOnlySettingsStore(SettingsScope.UserSettings);
        }
    }
}
