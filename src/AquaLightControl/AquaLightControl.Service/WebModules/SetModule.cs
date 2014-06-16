using AquaLightControl.Service.Devices;
using Nancy;

namespace AquaLightControl.Service.WebModules
{
    public sealed class SetModule : NancyModule
    {
        private readonly IDeviceController _device_controller;

        public SetModule(IDeviceController device_controller) {
            _device_controller = device_controller;
            Post["/set/{id:guid}/{value:range(0,65536)}"] = ctx => Set(ctx);
        }

        private dynamic Set(dynamic ctx) {

            //lock (_device_controller.SynchronizationLock) {
            //    var device = _device_controller.GetDevice(device_number);
            //    if (channel_number >= device.Channels.Count) {
            //        ThrowOnInvalidChannelNumber(channel_number);
            //    }

            //    unchecked {
            //        device.Channels.Set(channel_number, (ushort) pwm_value);
            //    }
            //}

            return HttpStatusCode.Accepted;
        }

        //private dynamic ThrowOnInvalidChannelNumber(int channel_number) {
        //    var message = string.Format("Invalid channel number {0}.", channel_number);
        //    return Response
        //        .AsText(message)
        //        .WithStatusCode(HttpStatusCode.NotAcceptable);
        //}
    }
}