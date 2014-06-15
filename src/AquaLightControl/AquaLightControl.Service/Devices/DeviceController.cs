using Raspberry.IO.Components.Controllers.Tlc59711;

namespace AquaLightControl.Service.Devices
{
    internal sealed class DeviceController : IDeviceController
    {
        private readonly IConnectionFactory _connection_factory;
        
       
        private readonly object _sync = new object();

        private ITlc59711Connection _connection;

        public int DeviceCount {
            get {
                return ReferenceEquals(_connection, null)
                    ? 0
                    : _connection.Devices.Count;
            }
        }

        public object SynchronizationLock {
            get { return _sync; }
        }

        public DeviceController(IConnectionFactory connection_factory) {
            _connection_factory = connection_factory;
        }

        public void Initialize() {
            lock (_sync) {
                
                if (!ReferenceEquals(_connection, null)) {
                    _connection.Dispose();
                    _connection = null;
                }
                
                _connection = _connection_factory.Establish();
            }
        }

        public ITlc59711Device GetDevice(int index) {
            lock (_sync) {
                if (ReferenceEquals(_connection, null)) {
                    throw new NotInitializedException();
                }

                return _connection.Devices[index];
            }
        }

        public void Update() {
            lock (_sync) {
                if (ReferenceEquals(_connection, null)) {
                    throw new NotInitializedException();
                }

                _connection.Update();
            }
        }
    }
}