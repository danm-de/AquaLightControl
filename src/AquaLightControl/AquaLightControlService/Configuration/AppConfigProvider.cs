using System.Configuration;

namespace AquaLightControlService.Configuration
{
    public sealed class AppConfigProvider : IConfigProvider {
        public string GetKey(string key_name) {
            return ConfigurationManager.AppSettings.Get(key_name);
        }
    }
}