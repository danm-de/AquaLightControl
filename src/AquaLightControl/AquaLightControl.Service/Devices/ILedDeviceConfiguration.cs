using System;

namespace AquaLightControl.Service.Devices
{
    public interface ILedDeviceConfiguration
    {
        void Save(Guid device_id, Device device);
        Device Get(Guid device_id);
        Device Delete(Guid device_id);
        Device[] GetAll();
    }
}