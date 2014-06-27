using System;
using System.Collections.Generic;

namespace AquaLightControl.ClientApi
{
    public interface IAquaLightConnection
    {
        string BaseUrl { get; set; }
        void Ping();
        IEnumerable<Device> GetAllDevices();
        void Save(Device device);
        void Delete(Guid device_id);
    }
}