using ServerAgent.Actor.Message;
using System;
using System.Net;

namespace ServerAgent.ActorLite
{
    public class HttpListenActor : ActorRefBase
    {
        private readonly HttpListener _httpListener;

        public HttpListenActor(string bindUrl)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"{bindUrl}");
        }

        protected override void OnStart()
        {
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
                    break;
            }
        }

        private void OnHttpContextMessage(HttpContextMessage message)
        {
            switch (message.HttpMethod)
            {
                case "GET":
                    OnGetMessage(message);
                    break;
                case "POST":
                    OnPostMessage(message);
                    break;
                case "PATCH":
                    OnPatchMessage(message);
                    break;
                case "PUT":
                    OnPutMessage(message);
                    break;
                case "DELETE":
                    OnDeleteMessage(message);
                    break;
                default:
                    Logger.Error($"request not found `{message.HttpMethod}:{message.RawUrl}`");
                    message.SendStatus(HttpStatusCode.NotFound);
                    break;
            }
        }

        protected virtual void OnGetMessage(HttpContextMessage message)
        {
            throw new NotImplementedException("Not Implemention `OnGetMessage`");
        }

        protected virtual void OnPostMessage(HttpContextMessage message)
        {
            throw new NotImplementedException("Not Implemention `OnPostMessage`");
        }

        protected virtual void OnPatchMessage(HttpContextMessage message)
        {
            throw new NotImplementedException("Not Implemention `OnPatchMessage`");
        }

        protected virtual void OnPutMessage(HttpContextMessage message)
        {
            throw new NotImplementedException("Not Implemention `OnPutMessage`");
        }

        protected virtual void OnDeleteMessage(HttpContextMessage message)
        {
            throw new NotImplementedException("Not Implemention `OnDeleteMessage`");
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

                var requestMessage = new HttpContextMessage()
                {
                    Request = context.Request,
                    Response = context.Response
                };
                Self.Tell(requestMessage);
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
