using System;

namespace AquaLightControl.Service.Devices
{
    public interface ILedDeviceConfiguration
    {
        void Save(Guid led_stripe_id, LedStripe led_stripe);
        LedStripe Get(Guid led_stripe_id);
        LedStripe Delete(Guid led_stripe_id);
        LedStripe[] GetAll();
    }
}