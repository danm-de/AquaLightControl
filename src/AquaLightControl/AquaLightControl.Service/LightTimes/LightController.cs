using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using AquaLightControl.Service.Clock;
using AquaLightControl.Service.Devices;
using AquaLightControl.Service.LightTimes.Factories;
using log4net;

namespace AquaLightControl.Service.LightTimes
{
    public sealed class LightController : ILightController, IDisposable
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(LightController));
        private readonly IClock _clock;
        private readonly ILedDeviceConfiguration _led_device_configuration;
        private readonly ILightConfigurationFactory _factory;

        private List<LightConfiguration> _light_configurations = new List<LightConfiguration>();
        private IDisposable _config_change_checker;

        public LightController(IClock clock, ILedDeviceConfiguration led_device_configuration, ILightConfigurationFactory factory) {
            _clock = clock;
            _led_device_configuration = led_device_configuration;
            _factory = factory;

            _config_change_checker = Observable.FromEventPattern<EventHandler<EventArgs>, EventArgs>(
                o => _led_device_configuration.ConfigurationChanged += o,
                o => _led_device_configuration.ConfigurationChanged -= o
                ).Subscribe(args => {
                    _logger.Debug("Reloading light configuration");
                    LoadConfiguration(); 
                });

            LoadConfiguration();
        }

        ~LightController() {
            Dispose(false);
        }

        private void LoadConfiguration() {
            var light_configurations = _led_device_configuration
                .GetAll()
                .Select(device => _factory.CreateLightConfiguration(device))
                .ToList();
                
            if (_logger.IsDebugEnabled) {
                light_configurations
                    .ForEach( config => _logger.DebugFormat("Found light configuration for device {0} channel {1} ({2})", 
                            config.DeviceNumber,
                            config.ChannelNumber,
                            config.Device.Name));
            }

            _light_configurations = light_configurations;
        }

        public LightResult SetLight(IDeviceController device_controller) {
            var time = _clock.GetTicksForTimeOfDay();
            var list = _light_configurations;

            var all_off = true;
            var has_changes = false;

            foreach (var config in list) {
                var device = device_controller.GetDevice(config.DeviceNumber);
                var old_value = device.GetPwmValue(config.Device);
                var new_value = unchecked((ushort)config.PowerCalculator.GetY(time));

                if (old_value != new_value) {
                    device.SetPwmValue(config.Device, new_value);
                    has_changes = true;
                }

                if (new_value > 0) {
                    all_off = false;
                }
            }
            
            return new LightResult(has_changes, all_off);
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            Trace.Assert(disposing,
                string.Format("ERROR: GC finalized {0} without calling Dispose()",
                    GetType()));

            if (!ReferenceEquals(_config_change_checker, null)) {
                _config_change_checker.Dispose();
                _config_change_checker = null;
            }
        }
    }
}