using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="operationMode")]
    public enum OperationMode
    {
        [EnumMember(Value = "testing")]
        Testing,
        [EnumMember(Value= "config")]
        Config
    }
}