using Nancy;

namespace AquaLightControl.Service.WebModules
{
    public sealed class PingModule : NancyModule
    {
        public PingModule() {
            
            Get["/ping"] = ctx => Response
                .AsText("pong")
                .WithStatusCode(HttpStatusCode.OK);

        }
    }
}