using System.Linq;
using System.Net;

namespace server_agent.Web.Controller
{
    public class ServerController : IController
    {
        private readonly IContext context;

        public ServerController(IContext context)
        {
            this.context = context;
        }

        [RouteAttribute(WebRequestMethods.Http.Put, @"\/server\/monitoring\/(on|off)")]
        public HttpListenerResponse PUT_Monitoring(HttpListenerRequest request, HttpListenerResponse response)
        {
            string param = request.Url.Segments.Last();
            switch (param)
            {
                case "on":
                    context.Monitoring = true;
                    break;
                case "off":
                    context.Monitoring = false;
                    break;
                default:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
            }

            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }
    }
}
