using AquaLightControl.Configuration;
using AquaLightControl.Service.LightTimes;
using AquaLightControl.Service.Relay;
using log4net;
using Raspberry.Timers;

namespace AquaLightControl.Service.Devices
{
    internal sealed class DeviceWorker : IDeviceWorker
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceWorker));
        private const string UPDATE_INTERVAL_KEY = "UpdateInterval";

        private readonly IConfigProvider _config_provider;
        private readonly IDeviceController _device_controller;
        private readonly ILightController _light_controller;
        private readonly IRelayService _relay_service;
        private readonly object _sync = new object();

        private bool _is_initialized;
        private long _update_interval;
        private bool _is_running;
        private ITimer _timer;
        private OperationMode _operation_mode = OperationMode.Normal;

        public OperationMode OperationMode {
            get { return _operation_mode; }
            set { _operation_mode = value; }
        }

        public DeviceWorker(IConfigProvider config_provider, IDeviceController device_controller, ILightController light_controller, IRelayService relay_service) {
            _config_provider = config_provider;
            _device_controller = device_controller;
            _light_controller = light_controller;
            _relay_service = relay_service;
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
                
                _timer = (_update_interval >= 10) ?
                    new StandardTimer()
                    : Timer.Create();
            
                _timer.Interval = _update_interval;
                _timer.Action = () => {
                    lock (_sync) {
                        if (OperationMode == OperationMode.Normal) {
                            var result = _light_controller.SetLight(_device_controller);

                            if (result.HasChanges) {
                                _device_controller.Update();
                            }

                            _relay_service.Turn(result.PowerOn);
                        } 
                    }
                };

                _timer.Start(0m);
                _logger.Info("Device worker has been started");
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
                _logger.Info("Device worker has been stopped");
            }
        }

        private void Initialize() {
            var update_interval_string = _config_provider.GetKey(UPDATE_INTERVAL_KEY);
            _update_interval = uint.Parse(update_interval_string);

            _logger.DebugFormat("Initialize device worker with an update interval of {0} ms", update_interval_string);
            _device_controller.Initialize();

            _is_initialized = true;
        }

        public void Dispose() {
            Stop();
        }
    }
}