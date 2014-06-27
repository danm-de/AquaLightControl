using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="dailyLightConfiguration")]
    public sealed class DailyLightConfiguration
    {
        [DataMember(Name = "lightsConfiguration")]
        public List<LightConfiguration> LightsConfiguration { get; set; }

    }
}