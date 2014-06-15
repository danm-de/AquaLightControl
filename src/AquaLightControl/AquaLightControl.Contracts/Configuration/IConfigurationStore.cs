namespace AquaLightControl.Configuration
{
    public interface IConfigStore
    {
         void Save<T>(string key, T @config);
        T Load<T>(string key);
    }
}