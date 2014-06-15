using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="lightConfiguration")]
    public sealed class LightConfiguration
    {
        [DataMember(Name= "ledStripe")]
        public LedStripe LedStripe { get; set; }
        
        [DataMember(Name = "lightTimes")]
        public LightTime[] LightTimes { get; set; }
    }
}