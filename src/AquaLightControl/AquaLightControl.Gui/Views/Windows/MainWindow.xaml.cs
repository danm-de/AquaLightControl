using System;
using System.Threading.Tasks;
using AquaLightControl.ClientApi;
using AquaLightControl.Gui.Configuration;
using AquaLightControl.Gui.ViewModels.Controls;
using AquaLightControl.Gui.ViewModels.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace AquaLightControl.Gui.Views.Windows
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, ILedDeviceViewer, IExceptionViewer
    {
        private readonly AquaLightConnection _connection;
        private readonly MainWindowViewModel _main_view_model;
        private UserSettingsStore _config;

        public MainWindow() {
            InitializeComponent();

            _config = new UserSettingsStore();
            _connection = new AquaLightConnection();

            _main_view_model = new MainWindowViewModel(_connection, _config, this, this);

            DataContext = _main_view_model;
        }
        
        public Task View(Exception exception) {
            return this.ShowMessageAsync("Fehler", exception.Message);
        }

        public Task View(Device device) {
            var dialog = (BaseMetroDialog)Resources["LedDeviceDialog"];
            
            var vm = new LedDeviceDialogViewModel(_connection) {
                CloseAction = () => { 
                    this.HideMetroDialogAsync(dialog);
                    _main_view_model.Refresh();
                }
            };
            vm.Initialize(device);

            dialog.DataContext = vm;
            return this.ShowMetroDialogAsync(dialog);
        }
    }
}
