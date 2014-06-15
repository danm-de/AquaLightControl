using System.IO;
using AquaLightControl.Configuration;
using AquaLightControl.Serialization;

namespace AquaLightControl.Service.Configuration
{
    public sealed class FileConfigurationStore : IConfigStore
    {
        private const string CONFIG_DIR = "config";

        private readonly object _sync = new object();
        private readonly ISerializer _serializer;

        public FileConfigurationStore(IJsonSerializerBuilder serializer_builder) {
            _serializer = serializer_builder.Build();
        }

        public void Save<T>(string key, T config) {
            CreateDirIfNotExists();
            lock (_sync) {
                using (var file = File.Create(GetFileName(key))) {
                    _serializer.Serialize(file, config);
                }
            }
        }

        public T Load<T>(string key) {
            CreateDirIfNotExists();
            
            lock (_sync) {
                var file_info = new FileInfo(GetFileName(key));
                if (!file_info.Exists) {
                    return default(T);
                }

                using (var file = file_info.OpenRead()) {
                    return _serializer.Deserialize<T>(file);
                }
            }
        }

        private void CreateDirIfNotExists() {
            var dir = new DirectoryInfo(CONFIG_DIR);
            
            if (dir.Exists) {
                return;
            }

            lock (_sync) {
                if (!dir.Exists) {
                    dir.Create();
                }
            }
        }

        private static string GetFileName(string key) {
            return Path.Combine(CONFIG_DIR, key);
        }
    }
}