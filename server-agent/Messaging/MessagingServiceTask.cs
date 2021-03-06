using log4net;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using ServerAgent.Data;
using System;
using System.Configuration;
using System.Threading.Tasks;

namespace ServerAgent.Messaging
{
    public class MessagingServiceTask : IServiceTask
    {
        private readonly ILog logger;
        private readonly IMessagingQueue handler;
        private bool isRunning;

        private Task publisherTask;

        public MessagingServiceTask(IMessagingQueue handler)
        {
            logger = LogManager.GetLogger(typeof(MessagingServiceTask));

            this.handler = handler;
            isRunning = false;
            publisherTask = null;
        }

        public void OnStart()
        {
            logger.Info("starting messaging service task");

            isRunning = true;
            publisherTask = Task.Run(() => PublisherTask());
        }

        public void OnStop()
        {
            logger.Info("stopping messaging service task");

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
                            Task.Delay(Const.MessageTaskWaitTime).Wait();
                            continue;
                        }

                        pubSocket.SendMoreFrame(item.Topic)
                            .SendFrame(JsonConvert.SerializeObject(item.Data));
                    }
                    catch (Exception ex)
                    {
                        logger?.Error("Exception - PublisherTask", ex);
                    }

                    Task.Delay(Const.MessageTaskWaitTime).Wait();
                }

                logger?.Info("closed Publisher socket");
            }
        }
    }
}
