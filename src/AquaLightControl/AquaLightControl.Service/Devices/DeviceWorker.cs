using AquaLightControl.Configuration;
using Raspberry.Timers;

namespace AquaLightControl.Service.Devices
{
    internal sealed class DeviceWorker : IDeviceWorker
    {
        private const string UPDATE_INTERVAL_KEY = "UpdateInterval";

        private readonly IConfigProvider _config_provider;
        private readonly IDeviceController _device_controller;
        private readonly object _sync = new object();

        private bool _is_initialized;
        private decimal _update_interval;
        private bool _is_running;
        private ITimer _timer;

        public DeviceWorker(IConfigProvider config_provider, IDeviceController device_controller) {
            _config_provider = config_provider;
            _device_controller = device_controller;
        }

        public void Start() {
            lock (_sync) {
                if (_is_running) {
                    throw new AlreadyStartedException();
                }

                if (!_is_initialized) {
                    Initialize();
                }

                _is_running = true;
                _timer = Timer.Create();
            
                _timer.Interval = _update_interval;
                _timer.Action = () => {
                    lock (_sync) {
                        _device_controller.Update();
                    }
                };

                _timer.Start(0m);
            }
        }

        public void Stop() {
            lock (_sync) {
                if (!_is_running) {
                    return;
                }

                _is_running = false;
                _timer.Stop();
                _timer = null;
            }
        }

        private void Initialize() {
            var update_interval_string = _config_provider.GetKey(UPDATE_INTERVAL_KEY);
            _update_interval = uint.Parse(update_interval_string) / 1000m;
            
            _device_controller.Initialize();

            _is_initialized = true;
        }

        public void Dispose() {
            Stop();
        }
    }
}