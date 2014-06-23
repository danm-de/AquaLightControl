using System;
using System.Collections.Generic;

namespace AquaLightControl.ClientApi
{
    public interface IAquaLightConnection
    {
        string BaseUrl { get; set; }
        void Ping();
        IEnumerable<LedStripe> GetAllStripes();
        void Save(LedStripe led_stripe);
        void Delete(Guid led_stripe_id);
    }
}