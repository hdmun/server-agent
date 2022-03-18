using Newtonsoft.Json;
using server_agent.Web.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;

namespace server_agent.Web.Controller
{
    public class ServerController : IController
    {
        private readonly IWebServiceContext context;

        public ServerController(IWebServiceContext context)
        {
            this.context = context;
        }

        [Route(WebRequestMethods.Http.Put, "/server/monitoring")]
        public HttpListenerResponse PUT_Monitoring(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                ServerMonitoringModel model = null;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                    model = JsonConvert.DeserializeObject<ServerMonitoringModel>(reader.ReadToEnd());

                if (model == null)
                {
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                context.Monitoring = model.On;
                response.StatusCode = (int)HttpStatusCode.OK; 
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }

        [Route(WebRequestMethods.Http.Put, @"\/server\/process\/(kill)")]
        public HttpListenerResponse PUT_Kill(HttpListenerRequest request, HttpListenerResponse response)
        {
            string param = request.Url.Segments.Last();
            switch (param)
            {
                case "kill":
                    context.OnServerKill();
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
