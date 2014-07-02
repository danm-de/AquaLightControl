using System.Threading.Tasks;
using AquaLightControl.Gui.Model;

namespace AquaLightControl.Gui
{
    public interface ILedDeviceViewer
    {
        Task View(LedDeviceModel device_model);
    }
}