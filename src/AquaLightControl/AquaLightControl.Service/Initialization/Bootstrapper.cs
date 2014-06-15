using Castle.Windsor;
using Nancy.Bootstrappers.Windsor;

namespace AquaLightControl.Service.Initialization
{
    public sealed class Bootstrapper : WindsorNancyBootstrapper
    {
        private readonly IWindsorContainer _container;

        public Bootstrapper(IWindsorContainer container) {
            _container = container;
        }

        protected override IWindsorContainer GetApplicationContainer() {
            return _container;
        }
    }
}