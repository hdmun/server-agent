using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using ServerAgent.Actor.Message;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace ServerAgent.ActorLite
{
    public class MessagePublishActor : ActorRefBase
    {
        private readonly string _bindAddr;
        private readonly ConcurrentQueue<PublishMessage> _messageQueue;

        private bool _isRunning;
        private SemaphoreSlim _semaphore;
        private Task _publisherTask;

        public MessagePublishActor(string bindAddr)
        {
            _bindAddr = bindAddr;
            _messageQueue = new ConcurrentQueue<PublishMessage>();

            _isRunning = false;
            _semaphore = new SemaphoreSlim(0, 1);
        }

        protected override void OnStart()
        {
            Logger.Info("starting messaging service task");

            _isRunning = true;
            _publisherTask = Task.Run(() => PublisherTask());
        }

        protected override void OnStop()
        {
            _isRunning = false;
            _publisherTask.Wait();

            Logger.Info("stopping messaging service task");
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case PublishMessage _message:
                    _messageQueue.Enqueue(_message);
                    _semaphore.Release(1);
                    break;
                default:
                    break;
            }
        }

        private void PublisherTask()
        {
            using (var pubSocket = new PublisherSocket())
            {
                Logger?.Info("Publisher socket binding...");

                pubSocket.Options.SendHighWatermark = 1000;
                pubSocket.Bind(_bindAddr);

                while (_isRunning)
                {
                    try
                    {
                        _semaphore.Wait();
                        
                        if (_messageQueue.IsEmpty)
                            continue;

                        if (!_messageQueue.TryDequeue(out PublishMessage message))
                            continue;

                        pubSocket.SendMoreFrame(message.Topic)
                            .SendFrame(JsonConvert.SerializeObject(message.Data));
                    }
                    catch (Exception ex)
                    {
                        Logger?.Error("Exception - `MessagePublishActor`", ex);
                    }
                }

                Logger?.Info("closed Publisher socket");
            }
        }
    }
}
