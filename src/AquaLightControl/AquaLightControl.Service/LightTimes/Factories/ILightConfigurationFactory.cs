namespace AquaLightControl.Service.LightTimes.Factories
{
    public interface ILightConfigurationFactory
    {
        LightConfiguration CreateLightConfiguration(Device device);
    }
}