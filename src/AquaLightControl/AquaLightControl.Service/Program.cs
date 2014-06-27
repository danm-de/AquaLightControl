using System;
using System.ServiceProcess;

namespace AquaLightControl.Service
{
    static class Program
    {
        private static void Main(string[] args) {
            ConfigureLogger();

            var service = new Service();

            if (Environment.UserInteractive) {
                service.Start(args);
                Console.WriteLine("Press any key to stop the program.");
                Console.ReadKey();
                service.Stop();
            } else {
                ServiceBase.Run(new ServiceBase[] {service});
            }
        }

        private static void ConfigureLogger() {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}
