using System;
using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Math.Helper;

namespace AquaLightControl.Math
{
    public class PowerCalculator : IPowerCalculator
    {
        private class Configuration
        {
            private readonly long _start;
            private readonly long _end;
            private readonly ILineFunction _line;
            
            public long Start {
                get { return _start; }
            }
            
            public long End {
                get { return _end; }
            }
            
            public ILineFunction Line {
                get { return _line; }
            }

            public Configuration(long start, long end, ILineFunction line_function) {
                _start = start;
                _end = end;
                _line = line_function;
            }
        }

        private readonly List<long> _configuration_start_times;
        private readonly List<Configuration> _configurations;
        private readonly long _start;
        private readonly long _end;

        public long Start {
            get { return _start; }
        }
        public long End {
            get { return _end; }
        }

        public PowerCalculator(ILineFunctionFactory factory, IEnumerable<Point> points) {
            var ordered_points = points
                .OrderBy(light_time => light_time.X)
                .ToArray();

            if (ordered_points.Length < 2) {
                throw new ArgumentException("Es muss mindestens zwei Punkte in der Zeitachse geben", "points");
            }

            var lines = new List<Tuple<Point, Point>>();
            for (var i = 0; i < (ordered_points.Length - 1); i++) {
                lines.Add(new Tuple<Point,Point>(ordered_points[i], ordered_points[i + 1]));
            }

            _configurations = lines
                .Select(line => CreateConfiguration(factory, line.Item1, line.Item2))
                .ToList();

            _configuration_start_times = _configurations
                .Select(c => c.Start)
                .ToList();

            _start = _configurations.First().Start;
            _end = _configurations.Last().End;
        }
        
        private static Configuration CreateConfiguration(ILineFunctionFactory factory, Point start, Point end) {
            var function = factory.Create(start, end);
            return new Configuration(start.X, end.X, function);
        }

        public long GetY(long x) {
            var index = _configuration_start_times.BinarySearch(x);
            if (index < 0) {
                index = ~index - 1;
            }

            if (index < 0) {
                x.ThrowArgumentOutOfRangeException(_start, _end);
            }

            var config = _configurations[index];
            
            if (config.End < x) {
                x.ThrowArgumentOutOfRangeException(_start, _end);
            }

            return config.Line.GetY(x);
        }
    }
}
