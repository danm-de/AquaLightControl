using System;
using System.Threading.Tasks;

namespace AquaLightControl.Gui
{
    public interface IExceptionViewer
    {
        Task View(Exception exception);
    }
}