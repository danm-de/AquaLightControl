using System;
using System.Threading.Tasks;
using AquaLightControl.ClientApi;
using AquaLightControl.Gui.Model;
using ReactiveUI;

namespace AquaLightControl.Gui.ViewModels
{
    public sealed class MainWindowViewModel : ReactiveObject
    {
        private readonly IAquaLightConnection _connection;
        private ConnectionState _connection_state;
        private string _base_url;
        private IExceptionViewer _exception_viewer;
        private ILedStripeViewer _led_stripe_viewer;

        public string BaseUrl {
            get { return _base_url; }
            set {
                this.RaiseAndSetIfChanged(ref _base_url, value);

                _connection.BaseUrl = _base_url;
                ConnectionState = ConnectionState.Unknown;
            }
        }

        public IExceptionViewer ExceptionViewer {
            get { return _exception_viewer; }
            set { this.RaiseAndSetIfChanged(ref _exception_viewer, value); }
        }

        public ILedStripeViewer LedStripeViewer {
            get { return _led_stripe_viewer; }
            set { this.RaiseAndSetIfChanged(ref _led_stripe_viewer, value); }
        }

        public ConnectionState ConnectionState {
            get { return _connection_state; }
            set { this.RaiseAndSetIfChanged(ref _connection_state, value); }
        }

        public IReactiveCommand CheckConnectionStateCommand { get; private set; }
        public IReactiveCommand AddLedStripeCommand { get; private set; }

        public MainWindowViewModel() {
            _connection = new AquaLightConnection();
            _base_url = _connection.BaseUrl;

            var base_url_is_set = this.WhenAny(vm => vm.BaseUrl, s => !string.IsNullOrWhiteSpace(s.Value));
            
            CheckConnectionStateCommand = new ReactiveCommand(base_url_is_set);
            CheckConnectionStateCommand
                .RegisterAsyncTask(_ => CheckConnectionState());
            CheckConnectionStateCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            AddLedStripeCommand = new ReactiveCommand(base_url_is_set);
            AddLedStripeCommand
                .RegisterAsyncTask(_ => AddAndSaveLedStripe());
            AddLedStripeCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

        }

        private async Task AddAndSaveLedStripe() {
            await _led_stripe_viewer.View(SaveLedStripe);
        }

        private async void SaveLedStripe(LedStripe led_stripe) {
            throw new NotImplementedException();
        }

        private async Task CheckConnectionState() {
            try {
                await _connection.Ping();
                ConnectionState = ConnectionState.Success;
            } catch (Exception) {
                ConnectionState = ConnectionState.Failed;
                throw;
            }
        }
    }
}