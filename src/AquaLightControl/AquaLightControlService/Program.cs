using System.ServiceProcess;

namespace AquaLightControlService
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        static void Main() {
            var services_to_run = new ServiceBase[]{ 
                new Service() 
            };

            ServiceBase.Run(services_to_run);
        }
    }
}
