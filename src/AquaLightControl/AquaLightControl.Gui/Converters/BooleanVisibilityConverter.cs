using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace AquaLightControl.Gui.Converters
{
    public sealed class BooleanVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type target_type, object parameter, CultureInfo culture) {
            if (ReferenceEquals(value, null) || !(value is bool)) {
                return DependencyProperty.UnsetValue;
            }

            return (bool) value
                ? Visibility.Visible
                : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type target_type, object parameter, CultureInfo culture) {
            if (ReferenceEquals(value, null) || !(value is Visibility)) {
                return false;
            }

            switch ((Visibility) value) {
                case  Visibility.Visible:
                    return true;
                default:
                    return false;
            }
        }
    }
}