using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Configuration;
using log4net;

namespace AquaLightControl.Service.Devices
{
    internal sealed class LedDeviceConfiguration : ILedDeviceConfiguration
    {
        private const string CONFIG_FILE_NAME = "devices";
        
        private readonly ILog _logger = LogManager.GetLogger(typeof(LedDeviceConfiguration));
        private readonly object _sync = new object();
        private readonly IConfigStore _store;

        private readonly Lazy<ConcurrentDictionary<Guid, Device>> _dict;
        
        public event EventHandler<EventArgs> ConfigurationChanged;

        public LedDeviceConfiguration(IConfigStore store) {
            _store = store;
            _dict = new Lazy<ConcurrentDictionary<Guid, Device>>(LoadFromStore);
        }

        public void Save(Guid device_id, Device device) {
            if (ReferenceEquals(device, null)) {
                throw new ArgumentNullException("device");
            }

            if (device.Id != device_id) {
                throw new ArgumentException("LED device id must match", "device");
            }

            _dict.Value.AddOrUpdate(device_id, device, (g, s) => device);
            
            _logger.InfoFormat("Configuration for device Id {0} (name: {1}, dev:{2}, channel: {3}) changed.", 
                device_id, 
                device.Name,
                device.DeviceNumber, 
                device.ChannelNumber
            );

            WriteToStore();
        }

        public Device Get(Guid device_id) {
            Device device;
            return _dict.Value.TryGetValue(device_id, out device)
                ? device
                : default(Device);
        }

        public Device[] GetAll() {
            return _dict.Value.Values.ToArray();
        }

        public Device Delete(Guid device_id) {
            Device device;
            if (_dict.Value.TryRemove(device_id, out device)) {
                _logger.InfoFormat("Delete configuration for device Id {0} (name: {1}, dev:{2}, channel: {3}).",
                    device_id,
                    device.Name,
                    device.DeviceNumber,
                    device.ChannelNumber
                );

                WriteToStore();
            }
            
            return device;
        }

        private ConcurrentDictionary<Guid, Device> LoadFromStore() {
            var devices = _store.Load<Device[]>(CONFIG_FILE_NAME);
            if (ReferenceEquals(devices, null)) {
                return new ConcurrentDictionary<Guid, Device>();
            }
            
            var kvps = devices
                .Select(s => new KeyValuePair<Guid, Device>(s.Id, s));

            return new ConcurrentDictionary<Guid, Device>(kvps);
        }

        private void WriteToStore() {
            var devices = GetAll();
            lock (_sync) {
                _store.Save(CONFIG_FILE_NAME, devices);
            }

            _logger.Debug("Device changes saved");

            OnConfigurationChanged();
        }

        private void OnConfigurationChanged() {
            var handler = ConfigurationChanged;
            if (handler != null) {
                handler(this, EventArgs.Empty);
            }
        }
    }
}