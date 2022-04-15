using log4net;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace ServerAgent.PubSub
{
    public class PubSubServiceTask : IServiceTask
    {
        private readonly ILog logger;
        private readonly IPubSubQueue handler;
        private bool isRunning;

        private Task publisherTask;

        public PubSubServiceTask(IPubSubQueue handler)
        {
            logger = LogManager.GetLogger(typeof(PubSubServiceTask));

            

            this.handler = handler;
            isRunning = false;
            publisherTask = null;
        }

        public void OnStart()
        {
            logger.Info("starting pub/sub service task");

            isRunning = true;
            publisherTask = Task.Run(() => PublisherTask());
        }

        public void OnStop()
        {
            logger.Info("stopping pub/sub service task");

            isRunning = false;
            Task.WaitAll(new Task[] { publisherTask });
        }

        private void PublisherTask()
        {
            using (var pubSocket = new PublisherSocket())
            {
                logger?.Info("Publisher socket binding...");

                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind(ConfigurationManager.AppSettings["PublisherAddr"]);

                while (isRunning)
                {
                    try
                    {
                        var item = handler.Dequeue();
                        if (item == null)
                        {
                            Task.Delay(1000).Wait();
                            continue;
                        }

                        pubSocket.SendMoreFrame(item.Topic)
                            .SendFrame(JsonConvert.SerializeObject(item.Data));
                    }
                    catch (Exception ex)
                    {
                        logger?.Error("Exception PublisherTask", ex);
                    }

                    Task.Delay(100).Wait();
                }
            }
        }
    }
}
