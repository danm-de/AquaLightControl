using System.IO;

namespace AquaLightControl.Serialization
{
    public interface ISerializer
    {
        string Serialize<T>(T instance);
        void Serialize<T>(TextWriter stream_writer, T instance);
        void Serialize<T>(Stream stream, T instance);

        T Deserialize<T>(string content);
        T Deserialize<T>(TextReader text_reader);
        T Deserialize<T>(Stream stream);
    }
}