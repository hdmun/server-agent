using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json.Linq;
using server_agent.PubSub;
using server_agent.PubSub.Model;
using System;
using System.Net;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class PubSubServiceTest : IPubSubQueue
    {
        [TestMethod]
        public void PublisherTask_Test()
        {
            var service = new PubSubService(this);

            MethodInfo onStart = typeof(PubSubService)
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

            MethodInfo onStop = typeof(PubSubService)
               .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }

        public PublishModel Dequeue()
        {
            return new PublishModel()
            {
                Topic = "ServerInfo",
                Data = JObject.FromObject(new ServerInfoModel()
                {
                    HostName = Dns.GetHostName(),
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
