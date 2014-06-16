using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Service.Devices;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AquaLightControl.Service.Install
{
    internal sealed class DeviceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(Registrations.ToArray());
        }

        private static IEnumerable<IRegistration> Registrations {
            get {
                yield return Component
                    .For<IConnectionFactory>()
                    .ImplementedBy<ConnectionFactory>()
                    .LifestyleSingleton();

                yield return Component
                    .For<IDeviceController>()
                    .ImplementedBy<DeviceController>()
                    .LifestyleSingleton();

                yield return Component
                    .For<IDeviceWorker>()
                    .ImplementedBy<DeviceWorker>()
                    .LifestyleSingleton();

                yield return Component
                    .For<ILedDeviceConfiguration>()
                    .ImplementedBy<LedDeviceConfiguration>()
                    .LifestyleSingleton();
            }
        }
    }
}