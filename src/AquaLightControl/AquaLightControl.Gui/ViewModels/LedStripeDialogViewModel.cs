using System;
using System.Windows.Media;
using ReactiveUI;

namespace AquaLightControl.Gui.ViewModels
{
    public sealed class LedStripeDialogViewModel : ReactiveObject
    {
        private const string COLOR_PROPERTY = "Color";
        private string _name;
        private int _device_number;
        private int _channel_number;
        private Guid _id = Guid.NewGuid();
        private bool _is_inverted;
        private byte _red;
        private byte _green;
        private byte _blue;
        private IReactiveCommand _save_command;
        private IReactiveCommand _cancel_command;
        private Action _close;
        private Action<LedStripe> _save;

        public Guid Id {
            get { return _id; }
            set { this.RaiseAndSetIfChanged(ref _id, value); }
        }

        public string Name {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        public int DeviceNumber {
            get { return _device_number; }
            set { this.RaiseAndSetIfChanged(ref _device_number, value); }
        }

        public int ChannelNumber {
            get { return _channel_number; }
            set { this.RaiseAndSetIfChanged(ref _channel_number, value); }
        }

        public bool IsInverted {
            get { return _is_inverted; }
            set { this.RaiseAndSetIfChanged(ref _is_inverted, value); }
        }

        public Brush Color {
            get { return new SolidColorBrush(System.Windows.Media.Color.FromRgb(Red, Green, Blue)); }
        }

        public byte Red {
            get { return _red; }
            set { 
                this.RaiseAndSetIfChanged(ref _red, value);
                raisePropertyChanged(COLOR_PROPERTY);
            }
        }

        public byte Green {
            get { return _green; }
            set { 
                this.RaiseAndSetIfChanged(ref _green, value);
                raisePropertyChanged(COLOR_PROPERTY);
            }
        }

        public byte Blue {
            get { return _blue; }
            set { 
                this.RaiseAndSetIfChanged(ref _blue, value);
                raisePropertyChanged(COLOR_PROPERTY);
            }
        }

        public IReactiveCommand SaveCommand {
            get { return _save_command; }
            private set { this.RaiseAndSetIfChanged(ref _save_command, value); }
        }

        public IReactiveCommand CancelCommand {
            get { return _cancel_command; }
            private set { this.RaiseAndSetIfChanged(ref _cancel_command, value); }
        }

        public Action Close {
            get { return _close; }
            set { this.RaiseAndSetIfChanged(ref _close, value); }
        }

        public Action<LedStripe> Save {
            get { return _save; }
            set { this.RaiseAndSetIfChanged(ref _save, value); }
        }

        private LedStripe Create() {
            return new LedStripe {
                Id = _id,
                Name = _name,
                DeviceNumber = _device_number,
                ChannelNumber = _channel_number,
                Invert = _is_inverted,
                Color = new RgbColor(_red, _green, _blue)
            };
        }
    }
}