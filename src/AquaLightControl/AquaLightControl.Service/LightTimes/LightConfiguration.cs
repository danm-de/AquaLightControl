using System;
using AquaLightControl.Math;

namespace AquaLightControl.Service.LightTimes
{
    public sealed class LightConfiguration
    {
        private readonly Device _device;
        private readonly IPowerCalculator _power_calculator;

        public int DeviceNumber {
            get { return _device.DeviceNumber; }
        }

        public int ChannelNumber {
            get { return _device.ChannelNumber; }
        }

        public IPowerCalculator PowerCalculator {
            get { return _power_calculator; }
        }

        public LightConfiguration(Device device, IPowerCalculator power_calculator) {
            if (ReferenceEquals(device, null)) {
                throw new ArgumentNullException("device");
            }
            if (ReferenceEquals(power_calculator, null)) {
                throw new ArgumentNullException("power_calculator");
            }

            _device = device;
            _power_calculator = power_calculator;
        }
    }
}