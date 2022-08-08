using log4net;
using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
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

        protected override void OnGetMessage(HttpContextMessage message)
        {
            switch (message.RawUrl)
            {
                case "/":
                    message.SendStatus(HttpStatusCode.OK);
                    break;
                default:
                    message.SendStatus(HttpStatusCode.NotFound);
                    break;
            }
        }

        protected override void OnPostMessage(HttpContextMessage message)
        {
            message.SendStatus(HttpStatusCode.NotFound);
        }

        protected override void OnPatchMessage(HttpContextMessage message)
        {
            switch (message.RawUrl)
            {
                case "/server/monitoring":
                    OnServerMonitoring(message);
                    break;
                default:
                    Logger.Error($"request not found `{message.RawUrl}`");
                    message.SendStatus(HttpStatusCode.NotFound);
                    break;
            }
        }

        protected override void OnDeleteMessage(HttpContextMessage message)
        {
            switch (message.RawUrl)
            {
                case "/server/process/kill":
                    OnServerProcessKill(message);
                    break;
                default:
                    Logger.Error($"request not found `{message.RawUrl}`");
                    message.SendStatus(HttpStatusCode.NotFound);
                    break;
            }
        }

        private void OnServerMonitoring(HttpContextMessage message)
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

        private void OnServerProcessKill(HttpContextMessage message)
        {
            try
            {
                var model = message.GetRequestBody<ServerKillRequest>();
                if (model == null)
                {
                    Logger.Error($"bad request body `{message.Url}`");
                    message.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }

                Logger.Info($"http request `{message.Url}`, {model.ServerName}, {model.KillCommand}");

                var askTask = _monitoringActor.Ask<ServerKillResponse>(new ServerKillRequest()
                {
                    KillCommand = model.KillCommand,
                    ServerName = model.ServerName,
                });

                var response = askTask.Result;

                Logger.Info($"http response `{message.Url}`, closed server count: {response.Servers.Length}");

                message.SendJson(HttpStatusCode.Created, response);
            }
            catch (Exception ex)
            {
                Logger.Error($"Exception `OnServerProcessKill`", ex);
                message.SendStatus(HttpStatusCode.InternalServerError);
            }
        }
    }
}
