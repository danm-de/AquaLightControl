using System;

namespace AquaLightControl.Service.Devices
{
    public interface IDeviceWorker : IDisposable
    {
        void Start();
        void Stop();
    }
}