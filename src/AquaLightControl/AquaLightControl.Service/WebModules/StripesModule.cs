using System;
using AquaLightControl.Service.Devices;
using Nancy;
using Nancy.ModelBinding;

namespace AquaLightControl.Service.WebModules
{
    public sealed class StripesModule : NancyModule
    {
        private readonly ILedDeviceConfiguration _device_configuration;

        public StripesModule(ILedDeviceConfiguration device_configuration) {
            _device_configuration = device_configuration;

            Post["/stripes"] = ctx => Save(ctx);
            Get["/stripes"] = ctx => LoadAll(ctx);
            Get["/stripes/{id:guid}"] = ctx => Load(ctx);
            Delete["/stripes/{id:guid}"] = ctx => Remove(ctx);
        }

        private dynamic Remove(dynamic ctx) {
            var led_stripe_id = (Guid) ctx.id;

            var deleted_entry = _device_configuration.Delete(led_stripe_id);

            if (ReferenceEquals(deleted_entry, null)) {
                return HttpStatusCode.NotFound;
            }

            return Response
                .AsJson(deleted_entry)
                .WithStatusCode(HttpStatusCode.OK);
        }
        
        private dynamic Load(dynamic ctx) {
            var led_stripe_id = (Guid)ctx.id;

            var led_stripe = _device_configuration.Get(led_stripe_id);
            
            if (ReferenceEquals(led_stripe, null)) {
                return HttpStatusCode.NotFound;
            }

            return Response
                .AsJson(led_stripe)
                .WithStatusCode(HttpStatusCode.OK);
        }

        private dynamic LoadAll(dynamic ctx) {
            return Response
                .AsJson(_device_configuration.GetAll())
                .WithStatusCode(HttpStatusCode.OK);
        }

        private dynamic Save(dynamic ctx) {
            var led_stripe = this.Bind<LedStripe>();

            if (ReferenceEquals(led_stripe, null)) {
                return Response
                    .AsText("Invalid JSON format for ledStripe.")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (led_stripe.Id == Guid.Empty) {
                return Response
                    .AsText("Unique LED stripe id required")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (led_stripe.DeviceNumber < 0) {
                return Response
                    .AsText("Device number must be greater or equal than 0")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (led_stripe.ChannelNumber < 0) {
                return Response
                    .AsText("Channel number must be greater or equal than 0")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            _device_configuration.Save(led_stripe.Id, led_stripe);

            var uri = string.Concat(Request.Url.SiteBase, Request.Path, "/", led_stripe.Id);
            
            var response = Response
                .AsJson(led_stripe)
                .WithStatusCode(HttpStatusCode.Created);
            
            response.Headers.Add("Location", uri);

            return response;
        }
    }
}