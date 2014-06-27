using System;
using System.Globalization;
using log4net;
using Nancy;
using AquaLightControl.Service.Devices;
using Raspberry.IO.Components.Controllers.Tlc59711;

namespace AquaLightControl.Service.WebModules
{
    public sealed class ValueModule : NancyModule
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(ValueModule));

        private readonly IDeviceController _device_controller;
        private readonly ILedDeviceConfiguration _device_configuration;

        public ValueModule(IDeviceController device_controller, ILedDeviceConfiguration device_configuration) {
            _device_controller = device_controller;
            _device_configuration = device_configuration;
            
            Get["/getValue/{id:guid}"] = ctx => GetValue(ctx);
            Put["/setValue/{id:guid}/{value:range(0,65535)}"] = ctx => SetValue(ctx);
        }

        private dynamic SetValue(dynamic ctx) {
            Func<dynamic, Device, ITlc59711Device,dynamic> action = (context, req_device, device) => {
                var value = unchecked((ushort) context.value);
                _logger.DebugFormat("Set PWM value of device {0},{1} to {2}", 
                    req_device.DeviceNumber,
                    req_device.ChannelNumber, 
                    value);
                device.Channels.Set(req_device.ChannelNumber, value);
                _device_controller.Update();
                return HttpStatusCode.Accepted;
            };

            return CheckAndRun(ctx, action);
        }

        private dynamic GetValue(dynamic ctx) {
            Func<dynamic, Device, ITlc59711Device, dynamic> action = (context, req_device, device) => {
                var value = device.Channels.Get(req_device.ChannelNumber);
                _logger.DebugFormat("Get PWM value of device {0},{1} ({2})",
                    req_device.DeviceNumber,
                    req_device.ChannelNumber,
                    value);
                return Response
                    .AsText(value.ToString(CultureInfo.InvariantCulture))
                    .WithStatusCode(HttpStatusCode.OK);
            };

            return CheckAndRun(ctx, action);
        }

        private dynamic CheckAndRun(dynamic ctx, Func<dynamic, Device, ITlc59711Device,dynamic> action) {
            Guid device_id = ctx.id;

            var requested_device = _device_configuration.Get(device_id);
            if (ReferenceEquals(requested_device, null)) {
                return Response
                    .AsText(string.Format("Device with id {0} not found.", device_id))
                    .WithStatusCode(HttpStatusCode.NotFound);
            }

            lock (_device_controller.SynchronizationLock) {
                if (_device_controller.DeviceCount <= requested_device.DeviceNumber) {
                    var error_message =
                        string.Format("Configuration failure for device {0}. Device with number {1} not available.",
                            device_id,
                            requested_device.DeviceNumber);
                    return Response
                        .AsText(error_message)
                        .WithStatusCode(HttpStatusCode.PreconditionFailed);
                }

                var device = _device_controller.GetDevice(requested_device.DeviceNumber);
                if (device.Channels.Count <= requested_device.ChannelNumber) {
                    var error_message =
                        string.Format(
                            "Configuration failure for device {0}. The device has only {1} channels (currently configured are {2}).",
                            device_id,
                            device.Channels.Count,
                            requested_device.ChannelNumber);
                    return Response
                        .AsText(error_message)
                        .WithStatusCode(HttpStatusCode.PreconditionFailed);
                }

                return action(ctx, requested_device, device);
            }
        }
    }
}