using Raspberry.IO.Components.Controllers.Tlc59711;

namespace AquaLightControl.Service.Devices
{
    public interface IConnectionFactory
    {
        ITlc59711Connection Establish();
    }
}