using System;
using System.Threading.Tasks;

namespace AquaLightControl.Gui
{
    public interface ILedStripeViewer
    {
        Task View(Action<LedStripe> save_command);
    }
}