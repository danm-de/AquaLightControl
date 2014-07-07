using System;
using System.Collections.Generic;
using System.Linq;
using AquaLightControl.Service.Relay;
using log4net;
using Nancy;
using AquaLightControl.Service.Devices;
using Nancy.ModelBinding;
using Raspberry.IO.Components.Controllers.Tlc59711;

namespace AquaLightControl.Service.WebModules
{
    public sealed class PwmSettingModule : NancyModule
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(PwmSettingModule));

        private readonly IDeviceController _device_controller;
        private readonly ILedDeviceConfiguration _device_configuration;
        private readonly IRelayService _relay_service;

        public PwmSettingModule(IDeviceController device_controller, ILedDeviceConfiguration device_configuration, IRelayService relay_service) {
            _device_controller = device_controller;
            _device_configuration = device_configuration;
            _relay_service = relay_service;

            Get["/devices/{id:guid}/pwm"] = ctx => GetValue(ctx);
            Put["/devices/{id:guid}/pwm"] = ctx => SetValue(ctx);
        }

        private dynamic SetValue(dynamic ctx) {
            Func<dynamic, Device, ITlc59711Device,dynamic> action = (context, req_device, device) => {
                var pwm_setting = this.Bind<PwmSetting>();
                if (ReferenceEquals(pwm_setting, null)) {
                    return Response
                        .AsText(string.Format("You need to specify PWM settings for device {0}.", req_device.Id))
                        .WithStatusCode(HttpStatusCode.BadRequest);
                }

                _logger.DebugFormat("Set PWM value of device {0},{1} to {2}", 
                    req_device.DeviceNumber,
                    req_device.ChannelNumber, 
                    pwm_setting.Value);
                device.SetPwmValue(req_device, pwm_setting.Value);
                _device_controller.Update();

                //var power_on = PowerOn(_device_configuration.GetAll());
                //_relay_service.Turn(power_on);

                return HttpStatusCode.Accepted;
            };

            return CheckAndRun(ctx, action);
        }

        private bool PowerOn(IEnumerable<Device> devices) {
            return devices.Select(device => _device_controller
                .GetDevice(device.DeviceNumber)
                .GetPwmValue(device))
                .Any(value => value > 0);
        }

        private dynamic GetValue(dynamic ctx) {
            Func<dynamic, Device, ITlc59711Device, dynamic> action = (context, req_device, device) => {
                var value = device.GetPwmValue(req_device);
                _logger.DebugFormat("Get PWM value of device {0},{1} ({2})",
                    req_device.DeviceNumber,
                    req_device.ChannelNumber,
                    value);

                var uri = string.Concat(Request.Url.SiteBase, Request.Path);
                var response = Response
                    .AsJson(new PwmSetting {
                        Value = value
                    })
                    .WithStatusCode(HttpStatusCode.OK);

                response.Headers.Add("Location", uri);

                return response;
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