using System;
using System.Collections.Generic;
using System.Linq;
using Raspberry.IO.Components.Controllers.Tlc59711;

namespace AquaLightControl.Service.Devices
{
    internal static class DeviceExtensionMethods
    {
        public static ushort GetPwmValue(this ITlc59711Device device, Device aqua_device) {
            var device_value = device.Channels.Get(aqua_device.ChannelNumber);

            return aqua_device.Invert
                ? (ushort)(UInt16.MaxValue - device_value)
                : device_value;
        }

        public static void SetPwmValue(this ITlc59711Device device, Device aqua_device, ushort value) {
            var new_value = aqua_device.Invert
                ? (ushort) (UInt16.MaxValue - value)
                : value;

            device.Channels.Set(aqua_device.ChannelNumber, new_value);
        }

        public static bool HasPwmValueGreaterThanZero(this IDeviceController device_controller, IEnumerable<Device> devices) {
            foreach (var device in devices) {
                var hardware_device = device_controller.GetDevice(device.DeviceNumber);
                var pwm_value = hardware_device.GetPwmValue(device);
                if (pwm_value > 0) {
                    return true;
                }
            }
            return false;
        }
    }
}