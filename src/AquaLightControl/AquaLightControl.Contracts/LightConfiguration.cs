using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AquaLightControl
{
    [DataContract(Name="lightConfiguration")]
    public sealed class LightConfiguration
    {
        [DataMember(Name = "dailyLightCurve")]
        public List<Point> DailyLightCurve { get; set; }
    }
}