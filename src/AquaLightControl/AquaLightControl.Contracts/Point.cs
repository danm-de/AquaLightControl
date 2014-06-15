using System;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="point")]
    public sealed class Point : IEquatable<Point>
    {
        private long _x;
        private long _y;

        private Point() {}

        public Point(long x, long y) {
            X = x;
            Y = y;
        }

        [DataMember(Name="x")]
        public long X {
            get { return _x; }
            private set { _x = value; }
        }

        [DataMember(Name = "y")]
        public long Y {
            get { return _y; }
            private set { _y = value; }
        }

        public bool Equals(Point other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }
            return _x == other._x && _y == other._y;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }
            if (ReferenceEquals(this, obj)) {
                return true;
            }
            return obj is Point && Equals((Point) obj);
        }

        public override int GetHashCode() {
            unchecked {
                return (_x.GetHashCode() * 397) ^ _y.GetHashCode();
            }
        }

        public static bool operator ==(Point left, Point right) {
            return Equals(left, right);
        }

        public static bool operator !=(Point left, Point right) {
            return !Equals(left, right);
        }
    }
}