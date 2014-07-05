using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using AquaLightControl.Service.Clock;
using AquaLightControl.Service.Devices;
using AquaLightControl.Service.ExtensionMethods;
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

        private Dictionary<Tuple<int, int>, LightConfiguration> _light_configurations = new Dictionary<Tuple<int, int>, LightConfiguration>();
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
                .ToDictionary(config => new Tuple<int, int>(config.DeviceNumber, config.ChannelNumber), config => config);

            if (_logger.IsDebugEnabled) {
                light_configurations
                    .ForEach(
                        kvp =>
                            _logger.DebugFormat("Found light configuration for device {0} channel {1}", kvp.Key.Item1,
                                kvp.Key.Item2));
            }

            _light_configurations = light_configurations;
        }

        public LightResult SetLight(IDeviceController device_controller) {
            var time = _clock.GetTicksForTimeOfDay();
            var dict = _light_configurations;

            var all_off = true;
            var has_changes = false;

            var device_count = device_controller.DeviceCount;
            for (var device_number = 0; device_number < device_count; device_number++) {
                var device = device_controller.GetDevice(device_number);
                var channel_count = device.Channels.Count;

                for (var channel = 0; channel < channel_count; channel++) {
                    LightConfiguration config;
                    var old_value = device.Channels[channel];
                    var new_value = old_value;
                    
                    if (dict.TryGetValue(new Tuple<int, int>(device_number, channel), out config)) {
                        new_value = unchecked((ushort)config.PowerCalculator.GetY(time));
                    }

                    if (old_value != new_value) {
                        has_changes = true;
                    }

                    if (new_value > 0) {
                        all_off = false;
                    }

                    device.Channels[channel] = new_value;
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