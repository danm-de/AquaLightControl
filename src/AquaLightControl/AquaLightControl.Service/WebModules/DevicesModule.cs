using System;
using AquaLightControl.Service.Devices;
using Nancy;
using Nancy.ModelBinding;

namespace AquaLightControl.Service.WebModules
{
    public sealed class DevicesModule : NancyModule
    {
        private readonly ILedDeviceConfiguration _device_configuration;

        public DevicesModule(ILedDeviceConfiguration device_configuration) {
            _device_configuration = device_configuration;

            Post["/devices"] = ctx => Save(ctx);
            Get["/devices"] = ctx => LoadAll(ctx);
            Get["/devices/{id:guid}"] = ctx => Load(ctx);
            Delete["/devices/{id:guid}"] = ctx => Remove(ctx);
        }

        private dynamic Remove(dynamic ctx) {
            var device_id = (Guid) ctx.id;

            var deleted_entry = _device_configuration.Delete(device_id);

            if (ReferenceEquals(deleted_entry, null)) {
                return HttpStatusCode.NotFound;
            }

            return Response
                .AsJson(deleted_entry)
                .WithStatusCode(HttpStatusCode.OK);
        }
        
        private dynamic Load(dynamic ctx) {
            var device_id = (Guid)ctx.id;

            var device = _device_configuration.Get(device_id);
            
            if (ReferenceEquals(device, null)) {
                return HttpStatusCode.NotFound;
            }

            return Response
                .AsJson(device)
                .WithStatusCode(HttpStatusCode.OK);
        }

        private dynamic LoadAll(dynamic ctx) {
            return Response
                .AsJson(_device_configuration.GetAll())
                .WithStatusCode(HttpStatusCode.OK);
        }

        private dynamic Save(dynamic ctx) {
            var device = this.Bind<Device>();

            if (ReferenceEquals(device, null)) {
                return Response
                    .AsText("Invalid JSON format for devices.")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (device.Id == Guid.Empty) {
                return Response
                    .AsText("Unique LED device id required")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (device.DeviceNumber < 0) {
                return Response
                    .AsText("Device number must be greater or equal than 0")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (device.ChannelNumber < 0) {
                return Response
                    .AsText("Channel number must be greater or equal than 0")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            _device_configuration.Save(device.Id, device);

            var uri = string.Concat(Request.Url.SiteBase, Request.Path, "/", device.Id);
            
            var response = Response
                .AsJson(device)
                .WithStatusCode(HttpStatusCode.Created);
            
            response.Headers.Add("Location", uri);

            return response;
        }
    }
}