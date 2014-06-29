using Raspberry.IO.Components.Controllers.Tlc59711;

namespace AquaLightControl.Service.Devices
{
    internal static class DeviceExtensionMethods
    {
        internal static ushort GetPwmValue(this ITlc59711Device device, Device aqua_device) {
            var device_value = device.Channels.Get(aqua_device.ChannelNumber);

            return aqua_device.Invert
                ? (ushort)(ushort.MaxValue - device_value)
                : device_value;
        }

        internal static void SetPwmValue(this ITlc59711Device device, Device aqua_device, ushort value) {
            var new_value = aqua_device.Invert
                ? (ushort) (ushort.MaxValue - value)
                : value;

            device.Channels.Set(aqua_device.ChannelNumber, new_value);
        }
    }
}