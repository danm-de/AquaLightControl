using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading.Tasks;
using AquaLightControl.ClientApi;
using AquaLightControl.ClientApi.Annotations;
using AquaLightControl.Configuration;
using AquaLightControl.Gui.Model;
using AquaLightControl.Gui.ViewModels.Controls;
using ReactiveUI;

namespace AquaLightControl.Gui.ViewModels.Windows
{
    public sealed class MainWindowViewModel : ReactiveObject, IDisposable
    {
        private const string CONFIG_REMOTE_ENDPOINT = "RemoteEndpoint";
        
        private readonly TimeSpan _set_pwm_value_delay = TimeSpan.FromMilliseconds(100);
        
        private readonly IAquaLightConnection _connection;
        private readonly IConfigStore _config_store;
        private readonly IExceptionViewer _exception_viewer;
        private readonly ILedDeviceViewer _led_device_viewer;

        private readonly ObservableCollection<LedDeviceModel> _led_devices = new ObservableCollection<LedDeviceModel>();

        private ConnectionState _connection_state;
        private string _base_url;
        private bool _test_mode;
        private ushort _selected_led_device_pwm_value;
        private IDisposable _change_pwm_value_disposable;
        private LightConfigurationViewModel _light_configuration_view_model;

        private LedDeviceModel _selected_led_device;
        private LedDeviceModel _selected_pwm_device;
        private IDisposable _change_pwm_device_disposable;
        private bool _show_only_selected_device;
        private bool _light_configuration_has_been_modified;
        private IDisposable _light_configuration_changed_disposable;
        private bool _show_tool_tips;

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

        public ObservableCollection<LedDeviceModel> LedDevices {
            get { return _led_devices; }
        }

        public LedDeviceModel SelectedLedDevice {
            get { return _selected_led_device; }
            set { 
                this.RaiseAndSetIfChanged(ref _selected_led_device, value);
                LightConfigurationViewModel.SelectedLedDevice = _selected_led_device;
            }
        }

        public LedDeviceModel SelectedPwmDevice {
            get { return _selected_pwm_device; }
            set { this.RaiseAndSetIfChanged(ref _selected_pwm_device, value); }
        }

        public ushort SelectedLedDevicePwmValue {
            get { return _selected_led_device_pwm_value; }
            set { this.RaiseAndSetIfChanged(ref _selected_led_device_pwm_value, value); }
        }

        public bool TestMode {
            get { return _test_mode; }
            set {
                SetOperationMode(value);
                this.RaiseAndSetIfChanged(ref _test_mode, value); 
            }
        }

        public bool ShowOnlySelectedDevice {
            get { return _show_only_selected_device; }
            set { 
                this.RaiseAndSetIfChanged(ref _show_only_selected_device, value);
                LightConfigurationViewModel.ShowOnlySelectedDevice = _show_only_selected_device;
            }
        }

        public bool LightConfigurationHasBeenModified {
            get { return _light_configuration_has_been_modified; }
            set { this.RaiseAndSetIfChanged(ref _light_configuration_has_been_modified, value); }
        }

        public LightConfigurationViewModel LightConfigurationViewModel {
            get { return _light_configuration_view_model; }
            set { this.RaiseAndSetIfChanged(ref _light_configuration_view_model, value); }
        }

        public bool ShowToolTips {
            get { return _show_tool_tips; }
            set { this.RaiseAndSetIfChanged(ref _show_tool_tips, value); }
        }

        public IReactiveCommand CheckConnectionStateCommand { get; private set; }
        public IReactiveCommand LedDeviceNewCommand { get; private set; }
        public IReactiveCommand LedDeviceEditCommand { get; private set; }
        public IReactiveCommand RefreshCommand { get; private set; }
        public IReactiveCommand SaveLightConfigurationsCommand { get; private set; }
        public IReactiveCommand ShowToolTipsCommand { get; private set; }

        public MainWindowViewModel() {}

        public MainWindowViewModel([NotNull] IAquaLightConnection connection, [NotNull] IConfigStore config_store, [NotNull] ILedDeviceViewer led_device_viewer, [NotNull] IExceptionViewer exception_viewer, [NotNull] LightConfigurationViewModel light_configuration_view_model) {
            if (ReferenceEquals(light_configuration_view_model, null)) {
                throw new ArgumentNullException("light_configuration_view_model");
            }
            if (ReferenceEquals(config_store, null)) {
                throw new ArgumentNullException("config_store");
            }
            if (ReferenceEquals(led_device_viewer, null)) {
                throw new ArgumentNullException("led_device_viewer");
            }
            if (ReferenceEquals(exception_viewer, null)) {
                throw new ArgumentNullException("exception_viewer");
            }
            if (ReferenceEquals(connection, null)) {
                throw new ArgumentNullException("connection");
            }
            
            _connection = connection;
            _config_store = config_store;
            _led_device_viewer = led_device_viewer;
            _exception_viewer = exception_viewer;
            _light_configuration_view_model = light_configuration_view_model;
            _base_url = _connection.BaseUrl;

            LightConfigurationViewModel.LedDevices = _led_devices;

            var is_base_url_set = this
                .WhenAny(vm => vm.BaseUrl, s => !string.IsNullOrWhiteSpace(s.Value));

            var connection_successful = this
                .WhenAny(vm => vm.ConnectionState, s => s.Value == ConnectionState.Success);

            CheckConnectionStateCommand = new ReactiveCommand(is_base_url_set);
            CheckConnectionStateCommand
                .Subscribe(_ => CheckConnectionState());
            CheckConnectionStateCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            var connection_established = is_base_url_set.CombineLatest(connection_successful, (s1, s2) => s1 && s2);

            RefreshCommand = new ReactiveCommand(connection_established);
            RefreshCommand
                .Subscribe(param => Refresh());
            RefreshCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            LedDeviceNewCommand = new ReactiveCommand(connection_established);
            LedDeviceNewCommand
                .RegisterAsyncTask(_ => ShowEmptyLedDeviceDialog());
            LedDeviceNewCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            var is_led_device_selected = this
                .WhenAny(vm => vm.SelectedLedDevice, s => !ReferenceEquals(s.Value, null));

            var edit_enabled = connection_established
                .CombineLatest(is_led_device_selected, (b1, b2) => b1 && b2);

            LedDeviceEditCommand = new ReactiveCommand(edit_enabled);
            #pragma warning disable 4014
            LedDeviceEditCommand
                .Subscribe(param => ShowLedDeviceDialog());
            #pragma warning restore 4014
            LedDeviceEditCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            var is_pwm_value_changed = this.WhenAny(model => model.SelectedLedDevicePwmValue, c => c.Value);
            var is_test_mode_enabled = this.WhenAny(vm => vm.TestMode, test_mode => test_mode);
            var is_pwm_device_selected = this.WhenAny(vm => vm.SelectedPwmDevice, c => c.Value);
            
            _change_pwm_value_disposable = is_pwm_value_changed
                .CombineLatest(is_pwm_device_selected, is_test_mode_enabled, (value, device, test_mode) => new {
                    Value = value,
                    Device = device,
                    TestMode = test_mode.Value
                })
                .Where(c => !ReferenceEquals(c.Device,null) && c.TestMode)
                .Throttle(_set_pwm_value_delay)
                .Where(c => c.Value == SelectedLedDevicePwmValue)
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(c => TryChangePwmValue(c.Device, c.Value));

            _change_pwm_device_disposable = is_pwm_device_selected
                .Where(device => !ReferenceEquals(device, null))
                .ObserveOn(DispatcherScheduler.Current)
                .Subscribe(device => TryRefreshPwmValue());

            SaveLightConfigurationsCommand = new ReactiveCommand(connection_established);
            SaveLightConfigurationsCommand
                .Subscribe(param => SaveLightConfiguration());
            SaveLightConfigurationsCommand.ThrownExceptions
                .Subscribe(exception => _exception_viewer.View(exception));

            ShowToolTipsCommand = new ReactiveCommand(connection_established);
            ShowToolTipsCommand
                .Subscribe(param => {
                    ShowToolTips = !ShowToolTips;
                    _light_configuration_view_model.ShowToolTips = ShowToolTips;
                });

            var light_configuration_has_been_changed = _light_configuration_view_model.WhenAny(vm => vm.HasModifiedItems, c => c.Value);
            _light_configuration_changed_disposable = light_configuration_has_been_changed.Subscribe(new_value => LightConfigurationHasBeenModified = new_value);

            LoadSettings();
        }

        private void SaveLightConfiguration() {
            var vm = _light_configuration_view_model;
            if (ReferenceEquals(vm, null)) {
                return;
            }

            vm.Update();
            _connection.SaveDevice(_led_devices.Select(m => m.Item));
        }

        private void LoadSettings() {
            var remote_endpoint = _config_store.Load<string>(CONFIG_REMOTE_ENDPOINT);
            if (!string.IsNullOrWhiteSpace(remote_endpoint)) {
                BaseUrl = remote_endpoint;
            }
        }

        private void SetOperationMode(bool value) {
            if (value == _test_mode) {
                return;
            }

            _connection.SetModeSettings(new ModeSettings {
                OperationMode = (value) ? OperationMode.Testing : OperationMode.Normal
            });
        }

        private void SaveSettings() {
            _config_store.Save(CONFIG_REMOTE_ENDPOINT, _base_url);
        }

        private void TryChangePwmValue(LedDeviceModel device, ushort value) {
            try {
                ChangePwmValue(device, value);
            } catch (Exception exception) {
                _exception_viewer.View(exception);
            }
        }

        private void ChangePwmValue(LedDeviceModel device, ushort value) {
            var selected_led_device = device;
            if (ReferenceEquals(selected_led_device, null)) {
                return;
            }

            _connection.SetPwmSetting(
                selected_led_device.Id,
                new PwmSetting {
                    Value = value
                }
            );
        }

        public void Refresh() {
            _led_devices.Clear();

            _connection
                .GetAllDevices()
                .Select(device => new LedDeviceModel(device))
                .OrderBy(m => m.Name)
                .ForEach(_led_devices.Add);

            TestMode = _connection
                .GetModeSettings()
                .OperationMode == OperationMode.Testing;

            RefreshPwmValue();
        }

        private void RefreshPwmValue() {
            var selected_pwm_device = SelectedPwmDevice;
            
            if (ReferenceEquals(selected_pwm_device, null)) {
                SelectedLedDevicePwmValue = 0;
                return;
            }

            SelectedLedDevicePwmValue = _connection
                .GetPwmSetting(selected_pwm_device.Id)
                .Value;
        }

        private void TryRefreshPwmValue() {
            try {
                RefreshPwmValue();
            } catch (Exception exception) {
                _exception_viewer.View(exception);
            }
        }

        private async Task ShowEmptyLedDeviceDialog() {
            await _led_device_viewer.View(null);
        }

        private async Task ShowLedDeviceDialog() {
            var led_device_model = SelectedLedDevice;
            
            if (ReferenceEquals(led_device_model, null)) {
                return;
            }

            await _led_device_viewer.View(led_device_model);
        }

        private void CheckConnectionState() {
            try {
                _connection.Ping();
                ConnectionState = ConnectionState.Success;
                
                Refresh();
                SaveSettings();
            } catch (Exception) {
                ConnectionState = ConnectionState.Failed;
                throw;
            }
        }

        public void Dispose() {
            if (!ReferenceEquals(_change_pwm_value_disposable, null)) {
                _change_pwm_value_disposable.Dispose();
                _change_pwm_value_disposable = null;
            }

            if (!ReferenceEquals(_change_pwm_device_disposable, null)) {
                _change_pwm_device_disposable.Dispose();
                _change_pwm_device_disposable = null;
            }

            if (!ReferenceEquals(_light_configuration_changed_disposable, null)) {
                _light_configuration_changed_disposable.Dispose();
                _light_configuration_changed_disposable = null;
            }
        }
    }
}