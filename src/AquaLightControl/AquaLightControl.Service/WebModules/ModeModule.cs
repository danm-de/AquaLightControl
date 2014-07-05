using AquaLightControl.Service.Devices;
using Nancy;
using Nancy.ModelBinding;

namespace AquaLightControl.Service.WebModules
{
    public sealed class ModeModule : NancyModule
    {
        private readonly IDeviceWorker _device_worker;

        public ModeModule(IDeviceWorker device_worker) {
            _device_worker = device_worker;

            Get["/mode"] = ctx => Response
                .AsJson(new ModeSettings {OperationMode = device_worker.OperationMode})
                .WithStatusCode(HttpStatusCode.OK);

            Put["/mode"] = ctx => SetValue(ctx);
        }

        private dynamic SetValue(dynamic ctx) {
            var mode_settings = this.Bind<ModeSettings>();

            if (ReferenceEquals(mode_settings, null)) {
                return Response
                     .AsText("You need to specify the operation mode.")
                     .WithStatusCode(HttpStatusCode.BadRequest);
            }

            _device_worker.OperationMode = mode_settings.OperationMode;

            return HttpStatusCode.Accepted;
        }
    }
}