using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="modeSettings")]
    public sealed class ModeSettings
    {
        [DataMember(Name="operationMode")]
        public OperationMode OperationMode { get; set; }
    }
}