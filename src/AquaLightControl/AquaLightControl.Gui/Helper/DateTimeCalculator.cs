using System;
using OxyPlot.Axes;

namespace AquaLightControl.Gui.Helper
{
    internal static class DateTimeCalculator
    {
        internal static readonly DateTime MagicDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        internal static readonly TimeSpan OneDay = TimeSpan.FromHours(24);

        internal static double GetMinimum() {
            return ConvertX(0);
        }

        internal static double GetMaximum() {
            return ConvertX(OneDay.Ticks);
        }

        internal static double ConvertX(this long ticks) {
            return DateTimeAxis.ToDouble(MagicDate.AddTicks(ticks));
        }

        internal static long ConvertX(this double x) {
            return DateTimeAxis.ToDateTime(x).Ticks - MagicDate.Ticks;
        }
    }
}