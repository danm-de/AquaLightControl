using System;
using System.Windows.Media;
using AquaLightControl.ClientApi.Annotations;

namespace AquaLightControl.Gui.Model
{
    public sealed class LedDeviceModel 
    {
        private readonly Device _led_device;

        public LedDeviceModel([NotNull] Device led_device) {
            if (ReferenceEquals(led_device, null)) {
                throw new ArgumentNullException("led_device");
            }

            _led_device = led_device;
        }

        public Device Item {
            get { return _led_device; }
        }

        public Guid Id {
            get { return _led_device.Id; }
        }

        public Brush Color {
            get { return new SolidColorBrush(System.Windows.Media.Color.FromRgb(_led_device.Color.Red, _led_device.Color.Green, _led_device.Color.Blue)); }
        }

        public string Name {
            get { return _led_device.Name; }
        }

        public int DeviceNumber {
            get { return _led_device.DeviceNumber; }
        }

        public int Channel {
            get { return _led_device.ChannelNumber; }
        }
    }
}