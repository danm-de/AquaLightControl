using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="lightConfiguration")]
    public sealed class LightConfiguration
    {
        [DataMember(Name= "deviceId")]
        public Guid DeviceId { get; set; }
        
        [DataMember(Name = "lightTimes")]
        public List<LightTime> LightTimes { get; set; }
    }
}