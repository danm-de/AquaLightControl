using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AquaLightControl.Gui.Controls.Views;
using AquaLightControl.Gui.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using ReactiveUI;

namespace AquaLightControl.Gui.Views
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, ILedStripeViewer, IExceptionViewer
    {
        public MainWindow() {
            InitializeComponent();
            var vm = new MainWindowViewModel();
            DataContext = vm;
            vm.ExceptionViewer = this;
            vm.LedStripeViewer = this;
        }
        
        public Task View(Exception exception) {
            return this.ShowMessageAsync("Fehler", exception.Message);
        }

        public Task View(Action<LedStripe> save_command) {
            var dialog = (BaseMetroDialog)Resources["LedStripeDialog"];
            
            var vm = new LedStripeDialogViewModel() {
                Save = save_command,
                Close = () => this.HideMetroDialogAsync(dialog)
            };

            dialog.DataContext = vm;
            return this.ShowMetroDialogAsync(dialog);
        }
    }
}
