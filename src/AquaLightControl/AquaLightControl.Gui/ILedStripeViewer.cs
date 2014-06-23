using System.Threading.Tasks;

namespace AquaLightControl.Gui
{
    public interface ILedStripeViewer
    {
        Task View(LedStripe led_stripe);
    }
}