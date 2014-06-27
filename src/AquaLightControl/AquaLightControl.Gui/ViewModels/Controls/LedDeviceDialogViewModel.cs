using System;
using System.Windows.Media;
using AquaLightControl.ClientApi;
using AquaLightControl.ClientApi.Annotations;
using ReactiveUI;

namespace AquaLightControl.Gui.ViewModels.Controls
{
    public sealed class LedDeviceDialogViewModel : ReactiveObject
    {
        private readonly IAquaLightConnection _connection;
        private const string COLOR_PROPERTY = "Color";
        private string _name;
        private int _device_number;
        private int _channel_number;
        private Guid _id;
        private bool _is_inverted;
        private byte _red = 255;
        private byte _green = 255;
        private byte _blue = 255;
        private Action _close_action;
        private string _exception_text;

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

        public string ExceptionText {
            get { return _exception_text; }
            set { 
                this.RaiseAndSetIfChanged(ref _exception_text, value);
                raisePropertyChanged("HasException");
            }
        }

        public bool HasException {
            get { return !string.IsNullOrWhiteSpace(_exception_text); }
        }

        public IReactiveCommand SaveCommand { get; private set; }
        public IReactiveCommand CancelCommand { get; private set; }
        public IReactiveCommand DeleteCommand { get; private set; }

        public Action CloseAction {
            get { return _close_action; }
            set { this.RaiseAndSetIfChanged(ref _close_action, value); }
        }

        public LedDeviceDialogViewModel([NotNull] IAquaLightConnection connection) {
            if (ReferenceEquals(connection, null)) {
                throw new ArgumentNullException("connection");
            }
            _connection = connection;

            var data_is_valid = this
                .WhenAny(vm => vm.Name, s => !string.IsNullOrWhiteSpace(s.Value));

            SaveCommand = new ReactiveCommand(data_is_valid);
            SaveCommand
                .Subscribe(_ => Save());
            SaveCommand.ThrownExceptions
                .Subscribe(ShowException);

            CancelCommand = new ReactiveCommand();
            CancelCommand
                .Subscribe(param => OnClose());

            var is_deletable = this
                .WhenAny(vm => vm.Id, id => id.Value != Guid.Empty);

            DeleteCommand = new ReactiveCommand(is_deletable);
            DeleteCommand
                .Subscribe(_ => Delete());
            DeleteCommand.ThrownExceptions
                .Subscribe(ShowException);

        }

        public void Initialize(Device led_device) {
            if (ReferenceEquals(led_device, null)) {
                return;
            }
            
            Id = led_device.Id;
            Name = led_device.Name;
            DeviceNumber = led_device.DeviceNumber;
            ChannelNumber = led_device.ChannelNumber;
            IsInverted = led_device.Invert;
            Red = led_device.Color.Red;
            Green = led_device.Color.Green;
            Blue = led_device.Color.Blue;
        }

        private void Delete() {
            _connection.Delete(_id);

            OnClose();
        }

        private void Save() {
            var led_device = Create();
           
            _connection.Save(led_device);
            
            OnClose();
        }

        private void OnClose() {
            var close_action = _close_action;
            if (ReferenceEquals(close_action, null)) {
                return;
            }
            close_action();
        }

        private void ShowException(Exception exception) {
            ExceptionText = exception.Message;
        }

        private Device Create() {
            return new Device {
                Id = (_id != Guid.Empty) ? _id : Guid.NewGuid(),
                Name = _name,
                DeviceNumber = _device_number,
                ChannelNumber = _channel_number,
                Invert = _is_inverted,
                Color = new RgbColor(_red, _green, _blue)
            };
        }
    }
}