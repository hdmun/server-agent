using log4net;
using Newtonsoft.Json;
using server_agent.Web.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace server_agent.Web.Controller
{
    public class ServerController : IController
    {
        private readonly ILog logger;
        private readonly IWebServiceContext context;

        public ServerController(IWebServiceContext context)
        {
            logger = LogManager.GetLogger(typeof(WebService));
            this.context = context;
        }

        [Route(WebRequestMethods.Http.Put, "/server/monitoring")]
        public HttpListenerResponse PUT_Monitoring(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                string inputData = null;
                ServerMonitoringModel model = null;
                using (var reader = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    inputData = reader.ReadToEnd();
                    model = JsonConvert.DeserializeObject<ServerMonitoringModel>(inputData);
                }

                if (model == null || inputData == null)
                {
                    logger.Error($"invalid request `/server/monitoring`");
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                if (model.HostName != Dns.GetHostName())
                {
                    logger.Error($"invalid HostName. {model.HostName}, {Dns.GetHostName()}");
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                context.Monitoring = model.On;
                response.StatusCode = (int)HttpStatusCode.Created;

                response.ContentType = "Application/json";
                byte[] buffer = Encoding.UTF8.GetBytes(inputData);
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
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
