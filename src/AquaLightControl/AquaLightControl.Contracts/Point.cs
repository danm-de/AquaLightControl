using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="point")]
    public sealed class Point 
    {
        public Point() {}
        public Point(long x, long y) {
            X = x;
            Y = y;
        }

        [DataMember(Name="x")]
        public long X { get; set; }

        [DataMember(Name = "y")]
        public long Y { get; set; }
    }
}