using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Configuration;

namespace AquaLightControl.Service.Devices
{
    internal sealed class LedDeviceConfiguration : ILedDeviceConfiguration
    {
        private const string CONFIG_FILE_NAME = "devices";
        private readonly object _sync = new object();
        private readonly IConfigStore _store;

        private readonly Lazy<ConcurrentDictionary<Guid, LedStripe>> _dict;

        public LedDeviceConfiguration(IConfigStore store) {
            _store = store;
            _dict = new Lazy<ConcurrentDictionary<Guid, LedStripe>>(LoadFromStore);
        }

        public void Save(Guid led_stripe_id, LedStripe led_stripe) {
            if (ReferenceEquals(led_stripe, null)) {
                throw new ArgumentNullException("led_stripe");
            }

            if (led_stripe.Id != led_stripe_id) {
                throw new ArgumentException("LED stripe id must match", "led_stripe");
            }

            _dict.Value.AddOrUpdate(led_stripe_id, led_stripe, (g, s) => led_stripe);
            WriteFromStore();
        }

        public LedStripe Get(Guid led_stripe_id) {
            LedStripe led_stripe;
            return _dict.Value.TryGetValue(led_stripe_id, out led_stripe)
                ? led_stripe
                : default(LedStripe);
        }

        public LedStripe[] GetAll() {
            return _dict.Value.Values.ToArray();
        }

        public LedStripe Delete(Guid led_stripe_id) {
            LedStripe stripe;
            _dict.Value.TryRemove(led_stripe_id, out stripe);
            return stripe;
        }

        private ConcurrentDictionary<Guid, LedStripe> LoadFromStore() {
            var led_stripes = _store.Load<LedStripe[]>(CONFIG_FILE_NAME);
            if (ReferenceEquals(led_stripes, null)) {
                return new ConcurrentDictionary<Guid, LedStripe>();
            }
            
            var kvps = led_stripes
                .Select(s => new KeyValuePair<Guid, LedStripe>(s.Id, s));

            return new ConcurrentDictionary<Guid, LedStripe>(kvps);
        }

        private void WriteFromStore() {
            var devices = GetAll();
            lock (_sync) {
                _store.Save(CONFIG_FILE_NAME, devices);
            }
        }
    }
}