using System.Collections.Generic;

namespace AquaLightControl.Math
{
    public interface IPowerCalculatorFactory
    {
        IPowerCalculator Create(IEnumerable<Point> points);
    }
}