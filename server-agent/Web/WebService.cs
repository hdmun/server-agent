using server_agent.Web.Controller;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace server_agent.Web
{
    public class WebService : ServiceBase
    {
        private readonly IList<IController> controllers;
        private readonly IRouter router;
        private readonly HttpListener httpListener;

        private Task taskJob;
        private bool isRunning;

        public WebService(IContext context)
        {
            controllers = new List<IController>()
            {
                new ServerController(context)
            };

            router = new Router();

            httpListener = new HttpListener();
            httpListener.Prefixes.Add("http://+:80/");

            taskJob = null;
            isRunning = false;
        }

        protected override void OnStart(string[] args)
        {
            Debug.WriteLine("WebService.OnStart");

            foreach (var controller in controllers)
            {
                router.Register(controller);
            }

            isRunning = true;
            taskJob = Task.Run(() => RunServer());
        }

        protected override void OnStop()
        {
            Debug.WriteLine("WebService.OnStop");

            isRunning = false;
            httpListener.Stop();
            Task.WaitAll(new Task[] { taskJob });
        }

        private void RunServer()
        {
            httpListener.Start();

            while (isRunning)
            {
                HttpListenerResponse response = null;
                try
                {
                    HttpListenerContext ctx = httpListener.GetContext();
                    response = ctx.Response;

                    int routing = router.Route(ctx);
                    if (routing <= 0)
                    {
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        response.Close();
                    }
                    response = null;
                }
                catch (HttpListenerException)
                {
                }
                catch (Exception)
                {
                    if (response != null)
                    {
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response?.Close();
                    }
                }
            }

            httpListener.Close();
        }
    }
}
