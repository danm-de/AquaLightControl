using System;
using System.Globalization;
using System.Windows;

namespace AquaLightControl.Gui.Converters
{
    public class EnumVisibilityConverter : EnumBooleanConverter
    {
        public override object Convert(object value, Type target_type, object parameter, CultureInfo culture) {
            var result = base.Convert(value, target_type, parameter, culture);

            var boolean_result = result as bool?;
            if (boolean_result.HasValue) {
                return boolean_result.Value
                    ? Visibility.Visible
                    : Visibility.Hidden;
            }

            return result;
        }
    }
}