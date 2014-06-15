using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="serviceConfiguration")]
    public sealed class ServiceConfiguration
    {
        [DataMember(Name="ledStripesConfiguration")]
        public LightConfiguration[] LightsConfiguration { get; set; }

    }
}