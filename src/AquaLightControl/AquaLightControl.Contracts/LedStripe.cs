namespace AquaLightControl
{
    public class LedStripe
    {
        public int DeviceNumber { get; set; }
        public int ChannelNumber { get; set; }
        public bool Invert { get; set; }
        public string Name { get; set; }

        public LightConfiguration[] LightTimes { get; set; }
    }
}