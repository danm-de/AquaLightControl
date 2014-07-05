using System;

namespace AquaLightControl.Service.Devices
{
    public interface IDeviceWorker : IDisposable
    {
        OperationMode OperationMode { get; set; }

        void Start();
        void Stop();
    }
}