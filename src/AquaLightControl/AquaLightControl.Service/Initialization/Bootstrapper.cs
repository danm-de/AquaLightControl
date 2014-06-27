using Castle.Windsor;
using Nancy.Bootstrappers.Windsor;
using Nancy.Diagnostics;

namespace AquaLightControl.Service.Initialization
{
    public sealed class Bootstrapper : WindsorNancyBootstrapper
    {
        private readonly IWindsorContainer _container;
        private readonly string _dashboard_password;

        public Bootstrapper(IWindsorContainer container) {
            _container = container;
        }

        public Bootstrapper(IWindsorContainer container, string dashboard_password) {
            _container = container;
            _dashboard_password = dashboard_password;
        }

        protected override IWindsorContainer GetApplicationContainer() {
            return _container;
        }

        protected override DiagnosticsConfiguration DiagnosticsConfiguration {
            get {
                return string.IsNullOrWhiteSpace(_dashboard_password)
                    ? base.DiagnosticsConfiguration
                    : new DiagnosticsConfiguration {Password = _dashboard_password};
            }
        }
    }
}