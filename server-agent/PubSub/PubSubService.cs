using log4net;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace server_agent.PubSub
{
    public class PubSubService : ServiceBase
    {
        private readonly ILog logger;
        private readonly IPubSubQueue handler;
        private bool isRunning;

        private Task publisherTask;

        public PubSubService(IPubSubQueue handler)
        {
            logger = LogManager.GetLogger(typeof(PubSubService));

            this.handler = handler;
            isRunning = false;
            publisherTask = null;
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("starting service");

            isRunning = true;
            publisherTask = Task.Run(() => PublisherTask());
        }

        protected override void OnStop()
        {
            logger.Info("stopping service");

            isRunning = false;
            Task.WaitAll(new Task[] { publisherTask });
        }

        private void PublisherTask()
        {
            using (var pubSocket = new PublisherSocket())
            {
                logger?.Info("Publisher socket binding...");

                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind("tcp://*:12345");

                while (isRunning)
                {
                    var item = handler.Dequeue();
                    if (item == null)
                    {
                        Task.Delay(1000).Wait();
                        continue;
                    }

                    pubSocket.SendMoreFrame(item.Topic)
                        .SendFrame(JsonConvert.SerializeObject(item));

                    Task.Delay(100).Wait();
                }
            }
        }
    }
}
