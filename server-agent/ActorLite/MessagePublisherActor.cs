using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using ServerAgent.Actor.Message;
using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ServerAgent.ActorLite
{
    public class MessagePublishActor : ActorRefBase
    {
        private readonly string _bindAddr;
        private readonly ConcurrentQueue<PublishMessage> _messageQueue;

        private bool _isRunning;
        private Task _publisherTask;

        public MessagePublishActor(string bindAddr)
        {
            _bindAddr = bindAddr;
            _messageQueue = new ConcurrentQueue<PublishMessage>();

            _isRunning = false;
        }

        protected override void OnStart()
        {
            // logger.Info("starting messaging service task");

            _isRunning = true;
            _publisherTask = Task.Run(() => PublisherTask());
        }

        protected override void OnStop()
        {
            _isRunning = false;
            _publisherTask.Wait();

            // logger.Info("stopping messaging service task");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case PublishMessage _message:
                    _messageQueue.Enqueue(_message);
                    break;
                default:
                    break;
            }
        }

        private void PublisherTask()
        {
            using (var pubSocket = new PublisherSocket())
            {
                // logger?.Info("Publisher socket binding...");

                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind(_bindAddr);

                while (_isRunning)
                {
                    try
                    {
                        if (_messageQueue.IsEmpty)
                        {
                            Task.Delay(1000).Wait();
                            continue;
                        }


                        if (!_messageQueue.TryDequeue(out PublishMessage message))
                            continue;

                        pubSocket.SendMoreFrame(message.Topic)
                            .SendFrame(JsonConvert.SerializeObject(message.Data));
                    }
                    catch (Exception)
                    {
                        // logger?.Error("Exception - PublisherTask", ex);
                    }

                    Task.Delay(500).Wait();
                }

                // logger?.Info("closed Publisher socket");
            }
        }
    }
}
