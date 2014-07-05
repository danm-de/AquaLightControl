using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Service.Clock;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AquaLightControl.Service.Install
{
    public sealed class ClockInstaller : IWindsorInstaller
    {
           public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(Registrations.ToArray());
        }

        private static IEnumerable<IRegistration> Registrations {
            get {
                yield return Component
                    .For<IReferenceClock>()
                    .ImplementedBy<LocalClock>()
                    .LifestyleSingleton();

                yield return Component
                    .For<IClock>()
                    .ImplementedBy<Clock.Clock>()
                    .LifestyleSingleton();
            }
        }
    }
}