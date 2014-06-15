using Castle.Windsor;

namespace AquaLightControl.Service.ServiceLocator
{
    internal sealed class ServiceLocatorImpl : IServiceLocator
    {
        private readonly IWindsorContainer _container;

        public ServiceLocatorImpl(IWindsorContainer container) {
            _container = container;
        }

        public IDisposableInstance<T> Resolve<T>() {
            var instance = _container.Resolve<T>();
            return new ResolvedInstance<T>(instance, i => _container.Release(i));
        }
    }
}