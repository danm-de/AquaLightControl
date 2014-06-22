using System.Collections.Generic;
using System.Threading.Tasks;

namespace AquaLightControl.ClientApi
{
    public interface IAquaLightConnection
    {
        string BaseUrl { get; set; }
        Task Ping();
        Task<IEnumerable<LedStripe>> GetAllStripes();
        Task Save(LedStripe led_stripe);
    }
}