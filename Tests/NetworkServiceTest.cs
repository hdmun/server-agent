using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using server_agent.Network;
using server_agent.Network.Model;
using server_agent.Network.Publish;
using System;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class NetworkServiceTest : INetworkHandler
    {
        [TestMethod]
        public void ResponseTask_Test()
        {
            var service = new NetworkService(this);

            MethodInfo onStart = typeof(NetworkService)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onStart.Invoke(service, new object[] { new string[] { } });

            using (var client = new RequestSocket())
            {
                client.Connect("tcp://localhost:5555");

                string reqMonitoring = "{\"type\":\"Monitoring\",\"data\":{\"on\":true}}";
                client.SendFrame(reqMonitoring);

                ResponseModel res = JsonConvert.DeserializeObject<ResponseModel>(client.ReceiveFrameString());
                Assert.AreEqual(res.Success, true);
                Assert.AreEqual(res.Message, "Success");
                Assert.AreEqual(res.Request, reqMonitoring);
            }

            MethodInfo onStop = typeof(NetworkService)
               .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }

        [TestMethod]
        public void PublisherTask_Test()
        {
            var service = new NetworkService(this);

            MethodInfo onStart = typeof(NetworkService)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onStart.Invoke(service, new object[] { new string[] { } });

            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://localhost:12345");
                subSocket.Subscribe("ServerInfo");

                string topic = subSocket.ReceiveFrameString();
                string serverInfo = subSocket.ReceiveFrameString();
                string topic2 = subSocket.ReceiveFrameString();
                string serverInfo2 = subSocket.ReceiveFrameString();
            }

            MethodInfo onStop = typeof(NetworkService)
               .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }

        public void OnRequest(IOutgoingSocket socket, RequestModel reqModel)
        {
            switch (reqModel.Type)
            {
                case Protocol.Monitoring:
                    socket.SendFrame(JsonConvert.SerializeObject(new ResponseModel()
                    {
                        Success = true,
                        Message = "Success",
                        Request = JsonConvert.SerializeObject(reqModel)
                    }));
                    break;
                default:
                    socket.SendFrame(JsonConvert.SerializeObject(new ResponseModel()
                    {
                        Success = false,
                        Message = "Invalid Protocol Message",
                        Request = JsonConvert.SerializeObject(reqModel)
                    }));
                    break;
            }
        }

        public PublishModel Dequeue()
        {
            return new PublishModel()
            {
                Topic = "ServerInfo",
                Data = JObject.FromObject(new ServerInfoModel()
                {
                    ProcessName = "Test ProcessName",
                    ServerName = "TestServer",
                    ProcessingTime = 3600,
                    ThreadId = 1000,
                    LastReceiveTime = DateTime.Parse("08/18/2018 07:22:16")
                })
            };
        }

        public void Enqueue(PublishModel item)
        {
            throw new NotImplementedException();
        }
    }
}
