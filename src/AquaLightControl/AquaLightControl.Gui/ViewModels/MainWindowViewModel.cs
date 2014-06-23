using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AquaLightControl.ClientApi;
using AquaLightControl.ClientApi.Annotations;
using AquaLightControl.Gui.Model;
using ReactiveUI;

namespace AquaLightControl.Gui.ViewModels
{
    public sealed class MainWindowViewModel : ReactiveObject
    {
        private readonly IAquaLightConnection _connection;
        private readonly IExceptionViewer _exception_viewer;
        private readonly ILedStripeViewer _led_stripe_viewer;
        
        private readonly ObservableCollection<LedStripeModel> _led_stripes = new ObservableCollection<LedStripeModel>();

        private ConnectionState _connection_state;
        private string _base_url;
        private LedStripeModel _selected_led_stripe;

        public string BaseUrl {
            get { return _base_url; }
            set {
                this.RaiseAndSetIfChanged(ref _base_url, value);

                _connection.BaseUrl = _base_url;
                ConnectionState = ConnectionState.Unknown;
            }
        }
        
        public ConnectionState ConnectionState {
            get { return _connection_state; }
            set { this.RaiseAndSetIfChanged(ref _connection_state, value); }
        }

        public ObservableCollection<LedStripeModel> LedStripes {
            get { return _led_stripes; }
        }

        public LedStripeModel SelectedLedStripe {
            get { return _selected_led_stripe; }
            set { this.RaiseAndSetIfChanged(ref _selected_led_stripe, value); }
        }

        public IReactiveCommand CheckConnectionStateCommand { get; private set; }
        public IReactiveCommand NewLedStripeCommand { get; private set; }
        public IReactiveCommand RefreshCommand { get; private set; }
        public IReactiveCommand EditLedStripeCommand { get; private set; }

        public MainWindowViewModel([NotNull] IAquaLightConnection connection, [NotNull] ILedStripeViewer led_stripe_viewer, [NotNull] IExceptionViewer exception_viewer) {
            if (ReferenceEquals(led_stripe_viewer, null)) {
                throw new ArgumentNullException("led_stripe_viewer");
            }
            if (ReferenceEquals(exception_viewer, null)) {
                throw new ArgumentNullException("exception_viewer");
            }
            if (ReferenceEquals(connection, null)) {
                throw new ArgumentNullException("connection");
            }

            _connection = connection;
            _led_stripe_viewer = led_stripe_viewer;
            _exception_viewer = exception_viewer;
            _base_url = _connection.BaseUrl;

            var is_base_url_set = this
                .WhenAny(vm => vm.BaseUrl, s => !string.IsNullOrWhiteSpace(s.Value));
            
            CheckConnectionStateCommand = new ReactiveCommand(is_base_url_set);
            CheckConnectionStateCommand
                .Subscribe(_ => CheckConnectionState());
            CheckConnectionStateCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            NewLedStripeCommand = new ReactiveCommand(is_base_url_set);
            NewLedStripeCommand
                .RegisterAsyncTask(_ => ShowEmptyLedStripeDialog());
            NewLedStripeCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            RefreshCommand = new ReactiveCommand();
            RefreshCommand
                .Subscribe(param => Refresh());
            RefreshCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            var is_led_stripe_selected = this
                .WhenAny(vm => vm.SelectedLedStripe, s => !ReferenceEquals(s.Value, null));

            var is_editable = is_base_url_set.CombineLatest(is_led_stripe_selected, (b1, b2) => b1 && b2);

            EditLedStripeCommand = new ReactiveCommand(is_editable);
            EditLedStripeCommand
                .Subscribe(param => ShowLedStripeDialog());
            EditLedStripeCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));
        }

        public void Refresh() {
            _led_stripes.Clear();

            _connection
                .GetAllStripes()
                .Select(led_stripe => new LedStripeModel(led_stripe))
                .OrderBy(m => m.Name)
                .ForEach(_led_stripes.Add);
        }

        private async Task ShowEmptyLedStripeDialog() {
            await _led_stripe_viewer.View(null);
        }

        private async Task ShowLedStripeDialog() {
            var led_stripe_model = SelectedLedStripe;
            
            if (ReferenceEquals(led_stripe_model, null)) {
                return;
            }

            await _led_stripe_viewer.View(led_stripe_model.Item);
        }

        private void CheckConnectionState() {
            try {
                _connection.Ping();
                ConnectionState = ConnectionState.Success;
            } catch (Exception) {
                ConnectionState = ConnectionState.Failed;
                throw;
            }
        }
    }
}