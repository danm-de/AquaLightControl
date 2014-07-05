using System;
using AquaLightControl.Gui;
using AquaLightControl.Math;
using AquaLightControl.Service.LightTimes.PowerCalculators;

namespace AquaLightControl.Service.LightTimes.Factories
{
    public sealed class LightConfigurationFactory : ILightConfigurationFactory
    {
        private static readonly TimeSpan _one_day = TimeSpan.FromHours(24);
        private readonly IPowerCalculatorFactory _power_calculator_factory;

        public LightConfigurationFactory(IPowerCalculatorFactory power_calculator_factory) {
            _power_calculator_factory = power_calculator_factory;
        }

        public LightConfiguration CreateLightConfiguration(Device device) {
            if (ReferenceEquals(device, null)) {
                throw new ArgumentNullException("device");
            }

            if (ReferenceEquals(device.LightConfiguration, null)
                || ReferenceEquals(device.LightConfiguration.DailyLightCurve, null)
                || device.LightConfiguration.DailyLightCurve.IsEmpty()) {
                return new LightConfiguration(device, new AlwaysOff(0, _one_day.Ticks));
            }

            var power_calculator = _power_calculator_factory.Create(device.LightConfiguration.DailyLightCurve);

            return new LightConfiguration(device, power_calculator);
        }
    }
}