using System.Configuration;
using AquaLightControl.Configuration;

namespace AquaLightControl.Service.Configuration
{
    public sealed class AppConfigProvider : IConfigProvider {
        public string GetKey(string key_name) {
            return ConfigurationManager.AppSettings.Get(key_name);
        }
    }
}