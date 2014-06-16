using System;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="ledStripe")]
    public sealed class LedStripe {
        [DataMember(Name="id")]
        public Guid Id { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }
        
        [DataMember(Name="deviceNumber")]
        public int DeviceNumber { get; set; }
        
        [DataMember(Name = "channelNumber")]
        public int ChannelNumber { get; set; }
        
        [DataMember(Name = "invert")]
        public bool Invert { get; set; }
    }
}