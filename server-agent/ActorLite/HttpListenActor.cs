using ServerAgent.Actor.Message;
using System;
using System.Net;
using System.Threading.Tasks;

namespace ServerAgent.ActorLite
{
    public class HttpListenActor : ActorRefBase
    {
        private readonly HttpListener _httpListener;
        private bool _isRunning;
        private Task _request;

        public HttpListenActor(string bindUrl)
        {
            _httpListener = new HttpListener();
            _httpListener.Prefixes.Add($"{bindUrl}");

            _isRunning = false;
        }

        protected override void OnStart()
        {
            _isRunning = true;
            _request = Task.Run(() => RunServer());
        }

        protected override void OnStop()
        {
            _isRunning = false;
            _request.Wait();
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
            }
            catch (HttpListenerException ex)
            {
                Logger?.Error("failed to start HttpListener", ex);
                return;
            }

            IAsyncResult asyncResult = null;
            while (_isRunning)
            {
                try
                {
                    if (!asyncResult?.IsCompleted ?? false)
                    {
                        Task.Delay(200).Wait();
                        continue;
                    }

                    asyncResult = _httpListener.BeginGetContext(GetContextCallback, _httpListener);
                }
                catch (Exception ex)
                {
                    Logger?.Error("exception in `HttpServerActor.RunServer`", ex);
                }
            }

            _httpListener.Close();
            Logger?.Info("closed HttpListener");
        }

        private void GetContextCallback(IAsyncResult ar)
        {
            HttpListenerContext context = null;
            try
            {
                context = _httpListener.EndGetContext(ar);

                if (!_isRunning)
                {
                    throw new Exception("Closed HttpServerActor");
                }

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
