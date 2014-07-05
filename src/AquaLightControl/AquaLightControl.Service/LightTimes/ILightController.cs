using AquaLightControl.Service.Devices;

namespace AquaLightControl.Service.LightTimes
{
    public interface ILightController
    {
        LightResult SetLight(IDeviceController device_controller);
    }
}