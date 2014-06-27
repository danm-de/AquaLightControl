using System.Threading.Tasks;

namespace AquaLightControl.Gui
{
    public interface ILedDeviceViewer
    {
        Task View(Device device);
    }
}