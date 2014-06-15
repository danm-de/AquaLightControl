using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AquaLightControl.Math
{
    public class PowerCalculator
    {
        public decimal Get(LightConfiguration config, long position) {
            if (ReferenceEquals(config, null)) {
                throw new ArgumentNullException("config");
            }
            if (position < config.Start || position > config.EndValue) {
                throw new ArgumentOutOfRangeException("position");
            }
        }
    }
}
