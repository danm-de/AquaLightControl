using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Configuration;
using AquaLightControl.Service.Configuration;
using AquaLightControl.Service.ServiceLocator;
using AquaLightControl.ServiceLocator;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Nancy.Bootstrappers.Windsor;

namespace AquaLightControl.Service.Install
{
    public sealed class ContainerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store) {
            container.AddFacility<TypedFactoryFacility>();
            container.Kernel.Resolver.AddSubResolver(new CollectionResolver(container.Kernel, true));
            container.Kernel.ProxyFactory.AddInterceptorSelector(new NancyRequestScopeInterceptorSelector());

            container.Register(Registrations(container).ToArray());
        }

        public static IEnumerable<IRegistration> Registrations(IWindsorContainer container) {
            yield return Component
                .For<IConfigProvider>()
                .ImplementedBy<AppConfigProvider>()
                .LifestyleSingleton();

            yield return Component
                .For<IServiceLocator>()
                .UsingFactoryMethod<IServiceLocator>(() => new ServiceLocatorImpl(container))
                .LifestyleSingleton();

            yield return Component
                .For<IWindsorContainer>()
                .Instance(container);

            yield return Component
                .For<NancyRequestScopeInterceptor>();
        }
    }
}