using System;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace AquaLightControl.Gui.Converters
{
    public class EnumBooleanConverter : IValueConverter
    {
        public virtual object Convert(object value, Type target_type, object parameter, CultureInfo culture) {
            if (ReferenceEquals(value, null)) {
                return DependencyProperty.UnsetValue;
            }

            var parameter_string = parameter as string;
            var value_type = value.GetType();

            if (string.IsNullOrWhiteSpace(parameter_string) || Enum.IsDefined(value_type, value) == false) {
                return DependencyProperty.UnsetValue;
            }

            var parameter_value = Enum.Parse(value_type, parameter_string);

            return parameter_value.Equals(value);
        }

        public virtual object ConvertBack(object value, Type target_type, object parameter, CultureInfo culture) {
            var parameter_string = parameter as string;
            
            return string.IsNullOrWhiteSpace(parameter_string)
                ? DependencyProperty.UnsetValue
                : Enum.Parse(target_type, parameter_string);
        }

    }
}