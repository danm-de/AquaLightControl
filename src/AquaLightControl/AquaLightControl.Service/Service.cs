using System;
using System.ServiceProcess;
using AquaLightControl.Configuration;
using AquaLightControl.Service.Initialization;
using AquaLightControl.Service.Install;
using AquaLightControl.ServiceLocator;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Microsoft.Owin.Hosting;
using Owin;

namespace AquaLightControl.Service
{
    public partial class Service : ServiceBase, IService
    {
        private const string DEFAULT_URL = "http://+:8080";
        private const string URL_KEY = "Url";

        private readonly IWindsorContainer _container;
        private readonly IServiceLocator _service_locator;
        
        private IDisposable _nancy_self_host;

        public Service() {
            InitializeComponent();
            
            _container = CreateWindsorContainer();
            _service_locator = _container.Resolve<IServiceLocator>();
        }

        private static IWindsorContainer CreateWindsorContainer() {
            var container = new WindsorContainer();
            container.Install(new IWindsorInstaller[] {new ContainerInstaller()});
            return container;
        }

        private void WebAppStartUp(IAppBuilder app) {
            app.UseNancy(options => {
                options.Bootstrapper = new Bootstrapper(_container);
            });
        }

        protected override void OnStart(string[] args) {
            string url;
            using (var config_provider = _service_locator.Resolve<IConfigProvider>()) {
                url = config_provider.Value.GetKey(URL_KEY) ?? DEFAULT_URL;
            }

            _nancy_self_host = WebApp.Start(url, WebAppStartUp);
        }

        protected override void OnStop() {
            if (ReferenceEquals(_nancy_self_host, null)) {
                return;
            }

            _nancy_self_host.Dispose();
            _nancy_self_host = null;
        }

        public void Start(string[] args) {
            OnStart(args);
        }
    }
}
