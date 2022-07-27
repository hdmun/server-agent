using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.Data.Model;
using System;
using System.Net;

namespace ServerAgent.Actor
{
    public class HttpServerActor : HttpListenActor
    {
        private readonly IActorRef _monitoringActor;

        public HttpServerActor(string bindUrl, IActorRef monitoringActor)
            : base(bindUrl)
        {
            _monitoringActor = monitoringActor;
        }

        protected override void OnGetMessage(HttpContextMessage message)
        {
            message.SendStatus(HttpStatusCode.NotFound);
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
                    message.SendStatus(HttpStatusCode.NotFound);
                    break;
            }
        }

        private void OnServerMonitoring(HttpContextMessage message)
        {
            try
            {
                var requestBody = message.GetRequestBody<ServerMonitoringRequest>();
                if (requestBody == null)
                {
                    // _logger.Error($"invalid request body`/server/monitoring`");
                    message.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }

                if (requestBody.HostName != Dns.GetHostName())
                {
                    // _logger.Error($"invalid request `HostName`. {model.HostName}, {Dns.GetHostName()}");
                    message.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }

                var askTask = _monitoringActor.Ask<MonitoringMessage>(
                    new MonitoringMessage()
                    {
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
            catch (Exception)
            {
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
                    // _logger.Error($"invalid request body `{request.Url}`");
                    message.SendStatus(HttpStatusCode.BadRequest);
                    return;
                }

                // _logger.Info($"http request `{request.Url}`, {model.ServerName}, {model.KillCommand}");

                var askTask = _monitoringActor.Ask<ServerKillResponse>(new ServerKillRequestMessage()
                {
                    KillCommand = model.KillCommand,
                    ServerName = model.ServerName,
                });

                var response = askTask.Result;

                // _logger.Info($"http response `{request.Url}`, closed server count: {closedServers.Length}");

                message.SendJson(HttpStatusCode.Created, response);
            }
            catch (Exception)
            {
                message.SendStatus(HttpStatusCode.InternalServerError);
            }
        }
    }
}
