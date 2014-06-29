using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name = "pwmSetting")]
    public sealed class PwmSetting
    {
        [DataMember(Name="value")]
        public ushort Value { get; set; }
    }
}