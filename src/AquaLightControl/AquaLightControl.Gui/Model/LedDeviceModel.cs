using System;
using System.Windows.Media;
using AquaLightControl.ClientApi.Annotations;
using ReactiveUI;

namespace AquaLightControl.Gui.Model
{
    public sealed class LedDeviceModel : ReactiveObject
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
            set {
                _led_device.Id = value;
                this.RaisePropertyChanged();
            }
        }

        public Brush Color {
            get {
                return
                    new SolidColorBrush(System.Windows.Media.Color.FromRgb(_led_device.Color.Red,
                        _led_device.Color.Green, _led_device.Color.Blue));
            }
        }

        public byte Red {
            get { return _led_device.Color.Red; }
            set {
                _led_device.Color.Red = value;
                this.RaisePropertyChanged();
            }
        }

        public byte Green {
            get { return _led_device.Color.Green; }
            set {
                _led_device.Color.Green = value;
                this.RaisePropertyChanged();
            }
        }

        public byte Blue {
            get { return _led_device.Color.Blue; }
            set {
                _led_device.Color.Blue = value;
                this.RaisePropertyChanged();
            }
        }

        public string Name {
            get { return _led_device.Name; }
            set {
                _led_device.Name = value;
                this.RaisePropertyChanged();
            }
        }

        public int DeviceNumber {
            get { return _led_device.DeviceNumber; }
            set {
                _led_device.DeviceNumber = value;
                this.RaisePropertyChanged();
            }
        }

        public int Channel {
            get { return _led_device.ChannelNumber; }
            set {
                _led_device.ChannelNumber = value;
                this.RaisePropertyChanged();
            }
        }

        public bool IsInverted {
            get { return _led_device.Invert; }
            set {
                _led_device.Invert = value;
                this.RaisePropertyChanged();
            }
        }

        public LightConfiguration LightConfiguration {
            get { return _led_device.LightConfiguration; }
            set {
                if (ReferenceEquals(value, null)) {
                    throw new ArgumentNullException("value");
                }

                if (ReferenceEquals(value.DailyLightCurve, null)) {
                    throw new ArgumentException("Die Eigenschaft DailyLightCurve darf nicht NULL sein.", "value");
                }

                _led_device.LightConfiguration = value;
                this.RaisePropertyChanged();
            }
        }
    }
}