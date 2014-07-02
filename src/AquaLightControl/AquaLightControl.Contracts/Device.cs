using System;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="device")]
    public sealed class Device {
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

        [DataMember(Name="color")]
        public RgbColor Color { get; set; }

        [DataMember(Name="lightConfiguration")]
        public LightConfiguration LightConfiguration { get; set; }
    }
}