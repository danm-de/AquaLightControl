using System.Configuration;

namespace AquaLightControl.Service.Configuration
{
    public sealed class AppConfigProvider : IConfigProvider {
        public string GetKey(string key_name) {
            return ConfigurationManager.AppSettings.Get(key_name);
        }
    }
}