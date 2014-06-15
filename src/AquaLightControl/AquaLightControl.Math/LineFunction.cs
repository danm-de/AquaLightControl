using System;

namespace AquaLightControl.Math
{
    public sealed class LineFunction : ILineFunction
    {
        private readonly long _x_offset;
        private readonly long _y_offset;
        private readonly double _c;
       
        public LineFunction(Point start, Point end) {
            if (start.X >= end.X) {
                throw new ArgumentException("Start X must lower than end X", "end");
            }

            _x_offset = start.X;
            _y_offset = start.Y;

            _c = (double)(end.Y - _y_offset) / (end.X - _x_offset);
        }

        public double GetY(long x) {
            return (x - _x_offset) * _c + _y_offset;
        }

        long ILineFunction.GetY(long x) {
            return unchecked ((long) GetY(x));
        }
    }
}