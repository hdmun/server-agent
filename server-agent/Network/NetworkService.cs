using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using server_agent.Network.Model;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace server_agent.Network
{
    public class NetworkService : ServiceBase
    {
        private readonly INetworkHandler handler;
        private bool isRunning;

        private Task responseTask;
        private Task publisherTask;

        public NetworkService(INetworkHandler handler)
        {
            this.handler = handler;
            isRunning = false;
            responseTask = null;
            publisherTask = null;
        }

        protected override void OnStart(string[] args)
        {
            Debug.WriteLine("NetworkService.OnStart");

            isRunning = true;
            responseTask = Task.Run(() => ResponseTask());
            publisherTask = Task.Run(() => PublisherTask());
        }

        protected override void OnStop()
        {
            Debug.WriteLine("NetworkService.OnStop");

            isRunning = false;
            Task.WaitAll(new Task[] { responseTask, publisherTask });
        }

        private void ResponseTask()
        {
            using (var server = new ResponseSocket())
            {
                server.Bind("tcp://*:5555");

                while (isRunning)
                {
                    string reqJson;
                    if (!server.TryReceiveFrameString(out reqJson))
                    {
                        Task.Delay(500).Wait();
                        continue;
                    }

                    try
                    {
                        var reqModel = JsonConvert.DeserializeObject<RequestModel>(reqJson);
                        handler.OnRequest(server, reqModel);
                    }
                    catch (Exception ex)
                    {
                        ResponseModel response = new ResponseModel()
                        {
                            Success = false,
                            Message = ex.Message,
                            Request = reqJson
                        };
                        server.SendFrame(JsonConvert.SerializeObject(response));
                    }
                }
            }
        }

        private void PublisherTask()
        {
            using (var pubSocket = new PublisherSocket())
            {
                Console.WriteLine("Publisher socket binding...");
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
