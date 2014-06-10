using System;
using System.ServiceProcess;

namespace AquaLightControlService
{
    static class Program
    {
        /// <summary>
        /// Der Haupteinstiegspunkt für die Anwendung.
        /// </summary>
        static void Main(string[] args) {
            var service = new Service();
        

            if (Environment.UserInteractive) {
                service.Start(args);
                Console.WriteLine("Press any key to stop the program.");
                Console.ReadKey();
                service.Stop();
            } else {
                ServiceBase.Run(new ServiceBase[]{ service});
            }
        }
    }
}
