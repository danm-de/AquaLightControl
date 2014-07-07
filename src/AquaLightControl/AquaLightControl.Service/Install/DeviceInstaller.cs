using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Math;
using AquaLightControl.Math.Factories;
using AquaLightControl.Service.Devices;
using AquaLightControl.Service.LightTimes;
using AquaLightControl.Service.LightTimes.Factories;
using AquaLightControl.Service.Relay;
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
                    .For<ILightController>()
                    .ImplementedBy<LightController>()
                    .LifestyleSingleton();

                yield return Component
                    .For<IDeviceWorker>()
                    .ImplementedBy<DeviceWorker>()
                    .LifestyleSingleton();

                yield return Component
                    .For<ILineFunctionFactory>()
                    .ImplementedBy<LineFunctionFactory>()
                    .LifestyleSingleton();

                yield return Component
                    .For<ILightConfigurationFactory>()
                    .ImplementedBy<LightConfigurationFactory>()
                    .LifestyleSingleton();

                yield return Component
                    .For<IPowerCalculatorFactory>()
                    .ImplementedBy<PowerCalculatorFactory>()
                    .LifestyleSingleton();

                yield return Component
                    .For<IRelayService>()
                    .ImplementedBy<RelayService>()
                    .LifestyleSingleton();

                yield return Component
                    .For<ILedDeviceConfiguration>()
                    .ImplementedBy<LedDeviceConfiguration>()
                    .LifestyleSingleton();
            }
        }
    }
}