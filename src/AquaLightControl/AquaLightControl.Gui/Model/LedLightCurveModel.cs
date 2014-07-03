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
        private const int HITBOX_LENGTH = 10;
        private const int NO_INDEX = -1;
        private const int MINIMUM_Y = 0;
        private const int MAXIMUM_Y = 65535;
        private readonly LedDeviceModel _led_device_model;
        private readonly LineSeries _line;

        private bool _is_disposed;
        private int _index_of_point_to_move = NO_INDEX;
        private bool _is_modified;

        public LineSeries Line {
            get { return _line; }
        }

        public Guid Id {
            get { return _led_device_model.Id; }
        }

        public bool IsModified {
            get { return _is_modified; }
            set { this.RaiseAndSetIfChanged(ref _is_modified, value); }
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

            _line.MouseDown += LineOnMouseDown;
            _line.MouseMove += LineOnMouseMove;
            _line.MouseUp += LineOnMouseUp;

            led_device_model.LightConfiguration.DailyLightCurve
                .OrderBy(point => point.X)
                .ForEach(point => _line.Points.Add(new DataPoint(point.X.ConvertX(), point.Y)));
        }

        private void LineOnMouseDown(object sender, OxyMouseDownEventArgs e) {
            if (e.ChangedButton != OxyMouseButton.Left && e.ChangedButton != OxyMouseButton.Right) {
                return;
            }

            int index_of_nearest_point = (int) Math.Round(e.HitTestResult.Index);
            var nearest_point = _line.Transform(_line.Points[index_of_nearest_point]);

            if ((nearest_point - e.Position).Length < HITBOX_LENGTH) {
                if (e.ChangedButton == OxyMouseButton.Right) {
                    // user wants to delete a point

                    var point = _line.Points[index_of_nearest_point];
                    if (Math.Abs(point.X - DateTimeCalculator.GetMinimum()) < double.Epsilon
                        || Math.Abs(point.X - DateTimeCalculator.GetMaximum()) < double.Epsilon) {
                        // we cannot delete the starting/ending points
                        return;
                    }

                    _line.Points.RemoveAt(index_of_nearest_point);
                    _line.PlotModel.InvalidatePlot(false);
                    e.Handled = true;
                    return;
                }
                
                // user clicked near to an existing data point -> move exiting point
                _index_of_point_to_move = index_of_nearest_point;
            } else {
                if (e.ChangedButton == OxyMouseButton.Right) {
                    // no point to delete within spitting distance 
                    return;
                }

                // user clicked near the line, a new point must be created
                _index_of_point_to_move = (int)e.HitTestResult.Index + 1;
                _line.Points.Insert(_index_of_point_to_move, _line.InverseTransform(e.Position));
            }

            _line.LineStyle = LineStyle.DashDot;
            _line.PlotModel.InvalidatePlot(false);

            e.Handled = true;
        }

        private void LineOnMouseMove(object sender, OxyMouseEventArgs e) {
            if (_index_of_point_to_move == NO_INDEX) {
                return;
            }

            var old_position = _line.Points[_index_of_point_to_move];
            var new_position = _line.InverseTransform(e.Position);

            var predecessors = _line.Points
                .Where(p => p.X < old_position.X)
                .OrderBy(p => p.X)
                .ToArray();

            var predecessor = (predecessors.Length == 0)
                ? (DataPoint?) null
                : predecessors.Last();

            var successors = _line.Points
                .Where(p => p.X > old_position.X)
                .OrderBy(p => p.X)
                .ToArray();

            var successor = (successors.Length == 0)
                ? (DataPoint?) null
                : successors.First();

            var minimum = DateTimeCalculator.GetMinimum();
            var maximum = DateTimeCalculator.GetMaximum();

            if ((predecessor.HasValue && new_position.X <= predecessor.Value.X) 
                || (successor.HasValue && new_position.X >= successor.Value.X)
                || new_position.X < minimum
                || new_position.X > maximum
                || Math.Abs(old_position.X - minimum) < double.Epsilon
                || Math.Abs(old_position.X - maximum) < double.Epsilon
                ) {
                new_position = new DataPoint(old_position.X, new_position.Y);
            }

            if (new_position.Y < MINIMUM_Y) {
                new_position = new DataPoint(new_position.X, MINIMUM_Y);
            }

            if (new_position.Y > MAXIMUM_Y) {
                new_position = new DataPoint(new_position.X, MAXIMUM_Y);
            }

            _line.Points[_index_of_point_to_move] = new_position;
            _line.PlotModel.InvalidatePlot(false);
            e.Handled = true;
        }

        private void LineOnMouseUp(object sender, OxyMouseEventArgs e) {
            if (_index_of_point_to_move == NO_INDEX) {
                return;
            }

            _index_of_point_to_move = NO_INDEX;
            _line.LineStyle = LineStyle.Solid;
            _line.PlotModel.InvalidatePlot(false);
            
            IsModified = true;

            e.Handled = true;
        }

        public void Update() {
            var points = _line.Points
                .Select(p => new Point(p.X.ConvertX(), (long) p.Y))
                .OrderBy(p => p.X);

            var daily_light_curve = _led_device_model.LightConfiguration.DailyLightCurve;

            daily_light_curve.Clear();
            daily_light_curve.AddRange(points);

            IsModified = false;
        }

        public void Dispose() {
            if (!_is_disposed) {
                _line.MouseDown -= LineOnMouseDown;
                _line.MouseMove -= LineOnMouseMove;
                _line.MouseUp -= LineOnMouseUp;
            }

            _is_disposed = true;
        }
    }
}