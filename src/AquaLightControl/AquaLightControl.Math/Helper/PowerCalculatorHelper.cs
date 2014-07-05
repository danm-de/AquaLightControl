using System;

namespace AquaLightControl.Math.Helper
{
    public static class PowerCalculatorHelper
    {
        public static void ThrowArgumentOutOfRangeException(this long x, long start, long end) {
            var message = String.Format("The value must be between {0} and {1}.", start, end);
            throw new ArgumentOutOfRangeException("x", x, message);
        }
    }
}