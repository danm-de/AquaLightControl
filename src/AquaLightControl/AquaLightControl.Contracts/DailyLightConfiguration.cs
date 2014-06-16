using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="dailyLightConfiguration")]
    public sealed class DailyLightConfiguration
    {
        [DataMember(Name = "lightsConfiguration")]
        public LightConfiguration[] LightsConfiguration { get; set; }

    }
}