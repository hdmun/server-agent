using log4net;
using ServerAgent.Web.Controller;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Threading.Tasks;

namespace ServerAgent.Web
{
    public class WebServiceTask : IServiceTask
    {
        private readonly ILog logger;

        private readonly IList<ControllerBase> controllers;
        private readonly IRouter router;
        private readonly string bindUrl;
        private readonly HttpListener httpListener;

        private Task taskJob;
        private bool isRunning;

        public WebServiceTask(IWebServiceContext context)
        {
            logger = LogManager.GetLogger(typeof(WebServiceTask));

            controllers = new List<ControllerBase>()
            {
                new ServerController(context)
            };

            router = new Router();

            bindUrl = ConfigurationManager.AppSettings["HttpUrl"];
            httpListener = new HttpListener();
            httpListener.Prefixes.Add($"{bindUrl}");

            taskJob = null;
            isRunning = false;
        }

        public void OnStart()
        {
            logger.Info("starting web service task");

            foreach (var controller in controllers)
            {
                router.Register(controller);
            }

            isRunning = true;
            taskJob = Task.Run(() => RunServer());
        }

        public void OnStop()
        {
            logger.Info("stopping web service task");

            isRunning = false;
            httpListener.Stop();
            Task.WaitAll(new Task[] { taskJob });
        }

        private void RunServer()
        {
            try
            {
                httpListener.Start();
                logger?.Info($"start HttpListener: {bindUrl}");
            }
            catch (HttpListenerException ex)
            {
                logger?.Error("failed to start HttpListener", ex);
                return;
            }

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
                catch (Exception ex)
                {
                    logger?.Error("Exception - WebServiceTaskJob", ex);

                    if (response != null)
                    {
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        response?.Close();
                    }
                }
            }

            httpListener.Close();
            logger?.Info("closed HttpListener");
        }
    }
}
