using System;
using System.Threading.Tasks;
using AquaLightControl.ClientApi;
using AquaLightControl.Gui.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace AquaLightControl.Gui.Views
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, ILedStripeViewer, IExceptionViewer
    {
        private readonly AquaLightConnection _connection;
        private readonly MainWindowViewModel _main_view_model;

        public MainWindow() {
            InitializeComponent();
            
            _connection = new AquaLightConnection();

            _main_view_model = new MainWindowViewModel(_connection, this, this);

            DataContext = _main_view_model;
        }
        
        public Task View(Exception exception) {
            return this.ShowMessageAsync("Fehler", exception.Message);
        }

        public Task View(LedStripe led_stripe) {
            var dialog = (BaseMetroDialog)Resources["LedStripeDialog"];
            
            var vm = new LedStripeDialogViewModel(_connection) {
                CloseAction = () => { 
                    this.HideMetroDialogAsync(dialog);
                    _main_view_model.Refresh();
                }
            };
            vm.Initialize(led_stripe);

            dialog.DataContext = vm;
            return this.ShowMetroDialogAsync(dialog);
        }
    }
}
