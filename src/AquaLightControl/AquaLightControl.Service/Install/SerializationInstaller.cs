using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Configuration;
using AquaLightControl.Serialization;
using AquaLightControl.Service.Configuration;
using AquaLightControl.Service.Serialization;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace AquaLightControl.Service.Install
{
    public sealed class SerializationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.Register(Registrations.ToArray());
        }

        private static IEnumerable<IRegistration> Registrations {
            get {
                yield return Component
                    .For<IJsonSerializerBuilder>()
                    .ImplementedBy<JsonSerializerBuilder>()
                    .LifestyleSingleton();

                yield return Component
                    .For<IConfigStore>()
                    .ImplementedBy<FileConfigurationStore>()
                    .LifestyleSingleton();
            }
        }
    }
}