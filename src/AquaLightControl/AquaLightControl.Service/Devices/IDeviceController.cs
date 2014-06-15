using Raspberry.IO.Components.Controllers.Tlc59711;

namespace AquaLightControl.Service.Devices
{
    public interface IDeviceController
    {
        int DeviceCount { get; }
      
        object SynchronizationLock { get; }

        void Initialize();
        void Update();
        
        ITlc59711Device GetDevice(int index);
    }
}