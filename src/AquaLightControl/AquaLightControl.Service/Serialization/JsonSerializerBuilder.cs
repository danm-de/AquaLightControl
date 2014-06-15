using AquaLightControl.Serialization;
using Newtonsoft.Json;

namespace AquaLightControl.Service.Serialization
{
    internal sealed class JsonSerializerBuilder : IJsonSerializerBuilder
    {
        public ISerializer Build() {
            var serializer = new JsonSerializer {
                TypeNameHandling = TypeNameHandling.None
            };

            var deserializer = new JsonSerializer {
                TypeNameHandling = TypeNameHandling.None
            };

            return new CustomJsonSerializer(serializer, deserializer);
        }
    }
}