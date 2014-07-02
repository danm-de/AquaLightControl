using System;
using System.Linq;
using AquaLightControl.ClientApi.Annotations;
using OxyPlot;
using OxyPlot.Wpf;
using ReactiveUI;

using LineSeries = OxyPlot.Series.LineSeries;
using DateTimeAxis = OxyPlot.Axes.DateTimeAxis;

namespace AquaLightControl.Gui.Model
{
    public sealed class LedLightCurveModel : ReactiveObject, IDisposable
    {
        private static readonly DateTime _magic_date = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Local);
        private readonly LedDeviceModel _led_device_model;
        private readonly LineSeries _line;

        public LineSeries Line {
            get { return _line; }
        }

        public Guid Id {
            get { return _led_device_model.Id; }
        }

        public LedLightCurveModel([NotNull] LedDeviceModel led_device_model) {
            if (ReferenceEquals(led_device_model, null)) {
                throw new ArgumentNullException("led_device_model");
            }
            _led_device_model = led_device_model;

            var color = led_device_model.Color.ToOxyColor();

            _line = new LineSeries {
                Color = color,
                MarkerFill = color,
                MarkerSize = 4,
                MarkerStroke = OxyColors.White,
                MarkerType = MarkerType.Circle,
                Title = led_device_model.Name
            };

            led_device_model.LightConfiguration.DailyLightCurve
                .OrderBy(point => point.X)
                .ForEach(point => _line.Points.Add(new DataPoint(ConvertX(point.X), point.Y)));
        }

        private static double ConvertX(long ticks) {
            return DateTimeAxis.ToDouble(_magic_date.AddTicks(ticks));
        }

        private static long ConvertX(double x) {
            return DateTimeAxis.ToDateTime(x).Ticks - _magic_date.Ticks;
        }

        private void Update() {
            var points = _line.Points
                .Select(p => new Point(ConvertX(p.X), (long) p.Y));

            var daily_light_curve = _led_device_model.LightConfiguration.DailyLightCurve;

            daily_light_curve.Clear();
            daily_light_curve.AddRange(points);
        }

        public void Dispose() {
            
        }
    }
}