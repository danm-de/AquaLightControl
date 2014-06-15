using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="lightTime")]
    public sealed class LightTime
    {
        [DataMember(Name="start")]
        public Point Start { get; set; }
        [DataMember(Name="end")]
        public Point End { get; set; }
    }
}