using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name = "lightTest")]
    public class LightTest
    {
        [DataMember(Name = "ledStripe")]
        public LedStripe LedStripe { get; set; }

        [DataMember(Name = "pwmValue")]
        public long PwmValue { get; set; }
    }
}