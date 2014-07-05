using System;

namespace AquaLightControl.Service.Devices
{
    public interface ILedDeviceConfiguration
    {
        event EventHandler<EventArgs> ConfigurationChanged; 
        void Save(Guid device_id, Device device);
        Device Get(Guid device_id);
        Device Delete(Guid device_id);
        Device[] GetAll();
    }
}