using System.IO;
using System.Text;
using AquaLightControl.Serialization;
using Newtonsoft.Json;

namespace AquaLightControl.Service.Serialization
{
    internal sealed class CustomJsonSerializer : ISerializer
    {
        private readonly JsonSerializer _serializer;
        private readonly JsonSerializer _deserializer;

        public CustomJsonSerializer(JsonSerializer serializer, JsonSerializer deserializer) {
            _serializer = serializer;
            _deserializer = deserializer;
        }

        public string Serialize<T>(T instance) {
            var sb = new StringBuilder();
            using (var writer = new StringWriter(sb)) {
                _serializer.Serialize(writer, instance);
                return sb.ToString();
            }
        }

        public void Serialize<T>(Stream stream, T instance) {
            using (var stream_writer = new StreamWriter(stream)) {
                Serialize(stream_writer, instance);
            }
        }

        public void Serialize<T>(TextWriter stream_writer, T instance) {
            using (var writer = new JsonTextWriter(stream_writer)) {
                _serializer.Serialize(writer, instance);
            }
        }

        public T Deserialize<T>(string content) {
            using (var string_reader = new StringReader(content)) {
                return Deserialize<T>(string_reader);
            }
        }

        public T Deserialize<T>(TextReader text_reader) {
            using (var reader = new JsonTextReader(text_reader)) {
                return _deserializer.Deserialize<T>(reader);
            }
        }

        public T Deserialize<T>(Stream stream) {
            using (var stream_reader = new StreamReader(stream)) {
                return Deserialize<T>(stream_reader);
            }
        }
    }
}