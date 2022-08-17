using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.ActorLite.Http;
using ServerAgent.ActorLite.Http.Route;
using System;
using System.Net;

namespace ServerAgent.Actor
{
    public class HttpServerActor : HttpListenActor
    {
        private readonly string _hostName = Dns.GetHostName();
        private readonly IActorRef _monitoringActor;

        public HttpServerActor(string bindUrl, IActorRef monitoringActor)
            : base(bindUrl)
        {
            _monitoringActor = monitoringActor;
        }

        [HttpGet("/")]
        public void GetHostState(HttpContextMessage message)
        {
            var askTask = _monitoringActor.Ask<HostStateResponse>(new HostStateRequest());
            var result = askTask.Result;
            if (askTask.Exception != null)
            {
                Logger.Error($"Exception `GetHostState`", askTask.Exception);
                message.SendStatus(HttpStatusCode.InternalServerError);
                return;
            }
            message.SendJson(HttpStatusCode.OK, result);
        }

        [HttpPatch("/monitoring")]
        public void UpdateMonitoring(HttpContextMessage message)
        {
            try
            {
                var requestBody = message.GetRequestBody<MonitoringMessage>();
                if (requestBody == null)
                {
                    Logger.Error($"bad request body`/server/monitoring`");
                    message.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }

                if (requestBody.HostName != _hostName)
                {
                    Logger.Error($"bad request `HostName`. {requestBody.HostName}, {_hostName}");
                    message.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }

                var askTask = _monitoringActor.Ask<MonitoringMessage>(
                    new MonitoringMessage()
                    {
                        HostName = _hostName,
                        On = requestBody.On
                    });

                var response = askTask.Result;
                if (response.On != requestBody.On)
                {
                    message.SendStatus(HttpStatusCode.InternalServerError);
                    return;
                }

                message.SendJson(HttpStatusCode.Created, response);
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception `OnServerMonitoring`", ex);
                message.SendStatus(HttpStatusCode.InternalServerError);
            }
        }

        [HttpGet("/process/{serverName:string}")]
        public void GetProcessState(HttpContextMessage message, string serverName)
        {
            try
            {
                var askTask = _monitoringActor.Ask<ProcessStateResponse>(new ProcessStateReqeuest()
                {
                    ServerName = serverName
                });

                var result = askTask.Result;
                if (askTask.Exception != null)
                {
                    Logger.Error($"Exception `GetProcessState` ProcessStateReqeuest", askTask.Exception);
                    message.SendStatus(HttpStatusCode.InternalServerError);
                    return;
                }
                message.SendJson(HttpStatusCode.OK, result);
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception `GetProcessState`", ex);
                message.SendStatus(HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete("/process/{serverName:string}/{killCommand:string}")]
        public void DeleteProcess(HttpContextMessage message, string serverName, string killCommand)
        {
            try
            {
                Logger.Info($"http request `{message.Url}`, {serverName}, {killCommand}");

                var askTask = _monitoringActor.Ask<ServerKillResponse>(new ServerKillRequest()
                {
                    ServerName = serverName,
                    KillCommand = killCommand,
                });

                var response = askTask.Result;

                Logger.Info($"http response `{message.Url}`, closed server count: {response.Servers?.Length}");

                message.SendJson(HttpStatusCode.OK, response);
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception `DeleteProcess`", ex);
                message.SendStatus(HttpStatusCode.InternalServerError);
            }
        }
    }
}
