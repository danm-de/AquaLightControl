using AquaLightControl.Configuration;

namespace AquaLightControl.Gui.Configuration
{
    public sealed class UserSettingsStore : IConfigStore
    {
        public void Save<T>(string key, T config) {
            Properties.Settings.Default[key] = config;
            Properties.Settings.Default.Save();
        }

        public T Load<T>(string key) {
            return (T)Properties.Settings.Default[key];
        }
    }
}