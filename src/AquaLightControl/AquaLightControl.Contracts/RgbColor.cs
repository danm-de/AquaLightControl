using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="rgbColor")]
    public sealed class RgbColor
    {
        private byte _red;
        private byte _green;
        private byte _blue;

        public RgbColor() {}

        public RgbColor(byte red, byte green, byte blue) {
            _red = red;
            _green = green;
            _blue = blue;
        }

        [DataMember(Name = "red")]
        public byte Red {
            get { return _red; }
            set { _red = value; }
        }

        [DataMember(Name = "green")]
        public byte Green {
            get { return _green; }
            set { _green = value; }
        }

        [DataMember(Name = "blue")]
        public byte Blue {
            get { return _blue; }
            set { _blue = value; }
        }
    }
}