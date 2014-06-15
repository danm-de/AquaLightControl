using System;
using System.Collections.Generic;
using System.Linq;

namespace AquaLightControl.Math
{
    public class PowerCalculator : ILineFunction
    {
        private readonly List<Configuration> _configurations;

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

        public PowerCalculator(ILineFunctionFactory factory, IEnumerable<LightTime> light_times) {
            _configurations = light_times
                .OrderBy(light_time => light_time.Start.X)
                .Select(light_time => CreateConfiguration(factory, light_time))
                .ToList();

            Check(_configurations);
        }

        private static void Check(IEnumerable<Configuration> configurations) {
            long? last_end = null;
            foreach (var config in configurations) {
                if (last_end.HasValue) {
                    if (config.Start != last_end.Value) {
                        var message = string.Format("Expected a start value of {0} but found {1}.", last_end.Value, config.Start);
                        throw new NonContinuousLightTimesException(message);
                    }
                }

                last_end = config.End;
            }

            if (!last_end.HasValue) {
                throw new LightTimesMissingException();
            }
        }

        private static Configuration CreateConfiguration(ILineFunctionFactory factory, LightTime light_time) {
            var start = light_time.Start.X;
            var end = light_time.End.X;
            var function = factory.Create(light_time);

            return new Configuration(start, end, function);
        }

        public long GetY(long x) {
            var config = _configurations
                .FirstOrDefault(c => c.Start <= x && c.End >= x);
            
            if (ReferenceEquals(config, null)) {
                var message = string.Format(
                    "The value must be between {0} and {1}.", 
                    _configurations.First().Start, 
                    _configurations.Last().End
                );
                throw new ArgumentOutOfRangeException("x", x, message);
            }

            return config.Line.GetY(x);
        }
    }
}
