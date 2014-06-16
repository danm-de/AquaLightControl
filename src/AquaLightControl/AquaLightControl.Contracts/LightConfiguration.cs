using System;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="lightConfiguration")]
    public sealed class LightConfiguration
    {
        [DataMember(Name= "ledStripeId")]
        public Guid LedStripeId { get; set; }
        
        [DataMember(Name = "lightTimes")]
        public LightTime[] LightTimes { get; set; }
    }
}