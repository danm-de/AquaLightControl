using System;
using System.ServiceProcess;
using AquaLightControl.Configuration;
using AquaLightControl.Service.Devices;
using AquaLightControl.Service.Initialization;
using AquaLightControl.Service.Install;
using AquaLightControl.ServiceLocator;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Nancy.Hosting.Self;

namespace AquaLightControl.Service
{
    public partial class Service : ServiceBase, IService
    {
        private const string DEFAULT_ENDPOINT = "http://localhost:8080";
        private const string CONFIG_KEY_ENDPOINT = "Endpoint";
        private const string CONFIG_KEY_DASHBOARD_PASSWORD = "DashboardPassword";

        private readonly ILog _logger = LogManager.GetLogger(typeof(Service));
        
        private readonly IWindsorContainer _container;
        private readonly IServiceLocator _service_locator;
        
        private IDisposableInstance<IDeviceWorker> _device_worker_disposable;
        private NancyHost _nancy_self_host;

        public Service() {
            InitializeComponent();
            
            _container = CreateWindsorContainer();
            _service_locator = _container.Resolve<IServiceLocator>();
        }

        private IWindsorContainer CreateWindsorContainer() {
            _logger.Debug("Create and initialize Windsor container");
            var container = new WindsorContainer();
            
            container.Install(new IWindsorInstaller[] {
                new ContainerInstaller(), 
                new SerializationInstaller(),
                new DeviceInstaller()
            });

            return container;
        }

        protected override void OnStart(string[] args) {
            string url, passwd;
            using (var config_provider = _service_locator.Resolve<IConfigProvider>()) {
                url = config_provider.Value.GetKey(CONFIG_KEY_ENDPOINT) ?? DEFAULT_ENDPOINT;
                passwd = config_provider.Value.GetKey(CONFIG_KEY_DASHBOARD_PASSWORD);
            }

            _logger.InfoFormat("Listening on {0}", url);

            if (IsRunningOnMono()) {
                _logger.Debug("Start device worker thread.");                
                _device_worker_disposable = _service_locator.Resolve<IDeviceWorker>();
                _device_worker_disposable.Value.Start();
            }

            _logger.DebugFormat("Start Nancy that will listen on {0}", url);
            _nancy_self_host = new NancyHost(new Bootstrapper(_container, passwd), new Uri(url));
            _nancy_self_host.Start();
            _logger.Debug("Nancy started");
        }

        protected override void OnStop() {
            if (ReferenceEquals(_nancy_self_host, null)) {
                return;
            }

            _logger.Debug("Stop Nancy");

            _nancy_self_host.Dispose();
            _nancy_self_host = null;

            if (IsRunningOnMono()) {
                _logger.Debug("Stop device worker thread");
                _device_worker_disposable.Value.Stop();
                _device_worker_disposable.Dispose();
            }
        }

        public void Start(string[] args) {
            OnStart(args);
        }

        public static bool IsRunningOnMono() {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
