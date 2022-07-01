using log4net;
using Newtonsoft.Json;
using ServerAgent.Web.Model;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ServerAgent.Web.Controller
{
    public class ServerController : ControllerBase
    {
        private readonly IWebServiceContext context;

        public ServerController(IWebServiceContext context)
            : base(typeof(WebServiceTask))
        {
            
            this.context = context;
        }

        [Route(WebRequestMethods.Http.Put, "/server/monitoring")]
        public HttpListenerResponse PUT_Monitoring(HttpListenerRequest request, HttpListenerResponse response)
        {
            try
            {
                var model = GetBody<ServerMonitoringModel>(request);
                if (model == null)
                {
                    _logger.Error($"invalid request body`/server/monitoring`");
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                if (model.HostName != Dns.GetHostName())
                {
                    _logger.Error($"invalid request `HostName`. {model.HostName}, {Dns.GetHostName()}");
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
                }

                context.Monitoring = model.On;
                response.StatusCode = (int)HttpStatusCode.Created;

                response.ContentType = "Application/json";
                byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(model));
                response.OutputStream.Write(buffer, 0, buffer.Length);
                response.OutputStream.Close();
            }
            catch (Exception)
            {
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

            return response;
        }

        [Route(WebRequestMethods.Http.Put, "/server/process/kill")]
        public HttpListenerResponse PUT_Kill(HttpListenerRequest request, HttpListenerResponse response)
        {
            var model = GetBody<ServerKillRequestModel>(request);
            if (model == null)
            {
                _logger.Error($"invalid request body `{request.Url}`");
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                return response;
            }

            _logger.Info($"http request `{request.Url}`, {model.ServerName}, {model.KillCommand}");

            ServerKillResponseModel[] closedServers;
            switch (model.KillCommand)
            {
                case "killAll":
                    {
                        closedServers = context.OnServerKill();
                        if (closedServers == null)
                            closedServers = new ServerKillResponseModel[] { };
                        break;
                    }
                case "closeAll":
                    {
                        closedServers = context.OnServerClose();
                        if (closedServers == null)
                            closedServers = new ServerKillResponseModel[] { };
                        break;
                    }
                case "kill":
                    {
                        var closedServer = context.OnServerKill(model.ServerName);
                        if (closedServer == null)
                            closedServers = new ServerKillResponseModel[] { };
                        else
                            closedServers = new ServerKillResponseModel[] { closedServer };
                        break;
                    }
                case "close":
                    {
                        var closedServer = context.OnServerClose(model.ServerName);
                        if (closedServer == null)
                            closedServers = new ServerKillResponseModel[] { };
                        else
                            closedServers = new ServerKillResponseModel[] { closedServer };
                        break;
                    }
                default:
                    response.StatusCode = (int)HttpStatusCode.BadRequest;
                    return response;
            }

            _logger.Info($"http response `{request.Url}`, closed server count: {closedServers.Length}");

            response.ContentType = "Application/json";
            byte[] buffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(closedServers));
            response.OutputStream.Write(buffer, 0, buffer.Length);
            response.OutputStream.Close();
            response.StatusCode = (int)HttpStatusCode.OK;
            return response;
        }
    }
}
