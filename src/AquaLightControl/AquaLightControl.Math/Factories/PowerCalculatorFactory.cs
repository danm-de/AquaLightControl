using System;
using System.Collections.Generic;

namespace AquaLightControl.Math.Factories
{
    public class PowerCalculatorFactory : IPowerCalculatorFactory
    {
        private readonly ILineFunctionFactory _line_function_factory;

        public PowerCalculatorFactory(ILineFunctionFactory line_function_factory) {
            _line_function_factory = line_function_factory;
        }

        public IPowerCalculator Create(IEnumerable<Point> points) {
            if (ReferenceEquals(points, null)) {
                throw new ArgumentNullException("points");
            }
            
            return new PowerCalculator(_line_function_factory, points);
        }
    }
}