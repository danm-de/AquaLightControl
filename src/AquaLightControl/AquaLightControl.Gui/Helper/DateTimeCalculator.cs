using System;
using OxyPlot.Axes;

namespace AquaLightControl.Gui.Helper
{
    internal static class DateTimeCalculator
    {
        internal static readonly TimeSpan Zero = TimeSpan.Zero;
        internal static readonly TimeSpan OneDay = TimeSpan.FromHours(24);

        internal static double GetMinimum() {
            return ConvertX(Zero.Ticks);
        }

        internal static double GetMaximum() {
            return ConvertX(OneDay.Ticks);
        }

        internal static double ConvertX(this long ticks) {
            var timespan = TimeSpan.FromTicks(ticks);
            return TimeSpanAxis.ToDouble(timespan);
        }

       internal static long ConvertX(this double x) {
            var timespan = TimeSpanAxis.ToTimeSpan(x);
           return timespan.Ticks;
       }
    }
}