using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="operationMode")]
    public enum OperationMode
    {
        [EnumMember(Value= "normal")]
        Normal = 0,
        [EnumMember(Value = "testing")]
        Testing = 1
    }
}