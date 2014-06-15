using AquaLightControl.Service.Devices;
using Nancy;
using Nancy.ModelBinding;

namespace AquaLightControl.Service.WebModules
{
    public sealed class TestModule : NancyModule
    {
        private readonly IDeviceController _device_controller;

        public TestModule(IDeviceController device_controller) {
            _device_controller = device_controller;
            Post["/test"] = ctx => Set(ctx);
        }

        private dynamic Set(dynamic ctx) {
            var test = this.Bind<LightTest>();
            if (ReferenceEquals(test, null)) {
                return Response
                    .AsText("Could not deserialize json.")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            if (ReferenceEquals(test.LedStripe, null)) {
                return Response
                    .AsText("No LED stipe specified.")
                    .WithStatusCode(HttpStatusCode.BadRequest);
            }

            var pwm_value = test.PwmValue;
            if (pwm_value < 0) {
                var message = string.Format("Invalid PWM value {0}.", pwm_value);
                return Response
                    .AsText(message)
                    .WithStatusCode(HttpStatusCode.NotAcceptable);
            }

            var device_number = test.LedStripe.DeviceNumber;
            if (device_number < 0 || device_number >= _device_controller.DeviceCount) {
                var message = string.Format("Invalid device number {0}.", device_number);
                return Response
                    .AsText(message)
                    .WithStatusCode(HttpStatusCode.NotAcceptable);
            }

            var channel_number = test.LedStripe.ChannelNumber;
            if (channel_number < 0) {
                return ThrowOnInvalidChannelNumber(channel_number);
            }

            lock (_device_controller.SynchronizationLock) {
                var device = _device_controller.GetDevice(device_number);
                if (channel_number >= device.Channels.Count) {
                    ThrowOnInvalidChannelNumber(channel_number);
                }

                unchecked {
                    device.Channels.Set(channel_number, (ushort) pwm_value);
                }
            }

            return HttpStatusCode.Accepted;
        }

        private dynamic ThrowOnInvalidChannelNumber(int channel_number) {
            var message = string.Format("Invalid channel number {0}.", channel_number);
            return Response
                .AsText(message)
                .WithStatusCode(HttpStatusCode.NotAcceptable);
        }
    }
}