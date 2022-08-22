using ServerAgent.Actor.Message;
using ServerAgent.ActorLite.Http.Route;
using System;
using System.Net;

namespace ServerAgent.ActorLite.Http
{
    public class HttpListenActor : ActorRefBase
    {
        private readonly HttpListener _httpListener;
        private readonly HttpRouter _router;

        public HttpListenActor(string bindUrl)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"{bindUrl}");
            _httpListener.AuthenticationSchemes = AuthenticationSchemes.IntegratedWindowsAuthentication;

            _router = new HttpRouter();
        }

        protected override void OnStart()
        {
            _router.Register(this);
            RunServer();
        }

        protected override void OnStop()
        {
            _httpListener.Close();
            Logger?.Info("closed HttpListener");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case HttpContextMessage _message:
                    OnHttpContextMessage(_message);
                    break;
                default:
                    // invalid message
                    string exceptMessage = $"Received message of type [{message.GetType()}] - Invalid message in HttpListenActor";
                    Logger.Error(exceptMessage);
                    throw new Exception(exceptMessage);  // todo. 커스텀 Exception 클래스 만들어서 처리
            }
        }

        private void OnHttpContextMessage(HttpContextMessage message)
        {
            if (!_router.Route(message))
            {
                message.SendStatus(HttpStatusCode.NotFound);
                Logger.Error($"request not found `{message.HttpMethod}` `{message.RawUrl}`");
            }
        }

        private void RunServer()
        {
            try
            {
                _httpListener.Start();
                Logger?.Info($"start HttpListener, Prefix Count: {_httpListener.Prefixes.Count}");
                foreach (var prefix in _httpListener.Prefixes)
                    Logger?.Info($"listening: {prefix}");

                _httpListener.BeginGetContext(GetContextCallback, _httpListener);
            }
            catch (HttpListenerException ex)
            {
                Logger?.Error("failed to start HttpListener", ex);
                return;
            }
        }

        private void GetContextCallback(IAsyncResult result)
        {
            HttpListenerContext context = null;
            try
            {
                if (!_httpListener.IsListening)
                    return;

                context = _httpListener.EndGetContext(result);
                _httpListener.BeginGetContext(GetContextCallback, result);

                Self.Tell(new HttpContextMessage(context));
            }
            catch (Exception ex)
            {
                Logger?.Error("exception in `HttpServerActor.GetContextCallback`", ex);

                if (context?.Response != null)
                {
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    context.Response?.Close();
                }
            }
        }
    }
}
