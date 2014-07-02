using System.Windows;
using System.Windows.Controls;
using AquaLightControl.Gui.ViewModels.Controls;

namespace AquaLightControl.Gui.Views.Controls
{
    /// <summary>
    /// Interaktionslogik für LineDrawControl.xaml
    /// </summary>
    public partial class LineDrawControl : UserControl
    {
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            "ViewModel", typeof(LightConfigurationViewModel), typeof(LineDrawControl), new PropertyMetadata(default(LightConfigurationViewModel), OnViewModelChanged));

        public LightConfigurationViewModel ViewModel {
            get { return (LightConfigurationViewModel) GetValue(ViewModelProperty); }
            set { 
                SetValue(ViewModelProperty, value);
                DataContext = value;
            }
        }

        private static void OnViewModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
            throw new System.NotImplementedException();
        }

        public LineDrawControl() {
            InitializeComponent();
        }
    }
}
