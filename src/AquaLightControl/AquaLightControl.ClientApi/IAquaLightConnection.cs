using System;
using System.Collections.Generic;

namespace AquaLightControl.ClientApi
{
    public interface IAquaLightConnection
    {
        string BaseUrl { get; set; }
        void Ping();
        
        IEnumerable<Device> GetAllDevices();
       
        void SaveDevice(Device device);
        void DeleteDevice(Guid device_id);

        PwmSetting GetPwmSetting(Guid device_id);
        void SetPwmSetting(Guid device_id, PwmSetting setting);
        
        ModeSettings GetModeSettings();
        void SetModeSettings(ModeSettings mode_settings);
    }
}