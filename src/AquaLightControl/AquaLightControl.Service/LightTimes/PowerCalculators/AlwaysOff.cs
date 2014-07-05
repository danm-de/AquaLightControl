using AquaLightControl.Math;
using AquaLightControl.Math.Helper;

namespace AquaLightControl.Service.LightTimes.PowerCalculators
{
    public class AlwaysOff : IPowerCalculator
    {
        public long Start { get; private set; }
        public long End { get; private set; }

        public AlwaysOff(long start, long end) {
            Start = start;
            End = end;
        }

        public long GetY(long x) {
            if (x < Start || x > End) {
                x.ThrowArgumentOutOfRangeException(Start, End);
            }
            return 0;
        }
    }
}