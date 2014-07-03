using System;
using System.Linq;
using AquaLightControl.ClientApi.Annotations;
using AquaLightControl.Gui.Helper;
using OxyPlot;
using OxyPlot.Wpf;
using ReactiveUI;

using LineSeries = OxyPlot.Series.LineSeries;

namespace AquaLightControl.Gui.Model
{
    public sealed class LedLightCurveModel : ReactiveObject, IDisposable
    {
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
                MarkerStroke = OxyColors.Black,
                MarkerType = MarkerType.Circle,
                Title = led_device_model.Name
            };

            led_device_model.LightConfiguration.DailyLightCurve
                .OrderBy(point => point.X)
                .ForEach(point => _line.Points.Add(new DataPoint(point.X.ConvertX(), point.Y)));
        }

        private void Update() {
            var points = _line.Points
                .Select(p => new Point(p.X.ConvertX(), (long) p.Y));

            var daily_light_curve = _led_device_model.LightConfiguration.DailyLightCurve;

            daily_light_curve.Clear();
            daily_light_curve.AddRange(points);
        }

        public void Dispose() {
            
        }
    }
}