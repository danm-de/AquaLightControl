﻿using System;
using System.Diagnostics;
using AquaLightControl.Configuration;
using AquaLightControl.Service.Devices;
using log4net;
using Raspberry.IO.GeneralPurpose;

namespace AquaLightControl.Service.Relay
{
    public sealed class RelayService : IRelayService, IDisposable
    {
        private const string GPIO_PIN_KEY = "RelayPin";

        private readonly ILog _logger = LogManager.GetLogger(typeof(DeviceController));

        private readonly ProcessorPin _pin;
        
        private FileGpioConnectionDriver _driver;
        private bool _power_on;
        private bool _disposed;

        public RelayService(IConfigProvider config_provider) {
            if (ReferenceEquals(config_provider, null)) {
                throw new ArgumentNullException("config_provider");
            }

            var gpio_pin_string = config_provider.GetKey(GPIO_PIN_KEY);

            int gpio_pin_value;
            if (!string.IsNullOrWhiteSpace(gpio_pin_string) && int.TryParse(gpio_pin_string, out gpio_pin_value)) {
                _pin = (ProcessorPin)gpio_pin_value;
                _logger.DebugFormat("Relay is connected at pin {0}", _pin);

                try {
                    _driver = new FileGpioConnectionDriver();
                    _driver.Allocate(_pin, PinDirection.Output);
                } catch (Exception exception) {
                    _logger.Fatal(exception.Message, exception);
                    _driver = null;
                }
            } else {
                _logger.Debug("No GPIO pin for relay configured");
            }
        }

        ~RelayService() {
            Dispose(false);
        }

        public void Turn(bool power_on) {
            if (power_on == _power_on) {
                return;
            }

            if (ReferenceEquals(_driver, null)) {
                return;
            }

            _power_on = power_on;
            
            try {
                _driver.Write(_pin, power_on);
                _logger.DebugFormat("Relay power {0}", (power_on)
                    ? "on"
                    : "off");
            } catch (Exception exception) {
                _logger.Fatal("Error when trying to switch relay: " + exception.Message, exception);
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing) {
            Trace.Assert(disposing, "Relay-Service has not been disposed.");
            
            if (!_disposed) {
                if (!ReferenceEquals(_driver, null)) {
                    _driver.Release(_pin);
                    _driver = null;
                }
            }

            _disposed = true;
        }
    }
}