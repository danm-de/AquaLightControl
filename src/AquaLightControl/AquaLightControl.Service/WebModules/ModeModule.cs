using Nancy;

namespace AquaLightControl.Service.WebModules
{
    public sealed class ModeModule : NancyModule
    {
         public ModeModule() {

             Get["/mode"] = ctx => Response
                .AsJson(new ModeSettings { OperationMode = OperationMode.Testing})
                .WithStatusCode(HttpStatusCode.OK);

             Put["/mode"] = ctx => Response
                 .AsText("Nicht implementiert")
                 .WithStatusCode(HttpStatusCode.NotImplemented);

         }
    }
}