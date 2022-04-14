using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerAgent.PubSub;
using ServerAgent.PubSub.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class PubSubServiceTest : IPubSubQueue
    {
        private Queue<PublishModel> publishQueue = new Queue<PublishModel>();

        [TestMethod]
        public void PublisherTask_ServerInfoTest()
        {
            var service = new PubSubServiceTask(this);

            MethodInfo onStart = typeof(PubSubServiceTask)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onStart.Invoke(service, new object[] { new string[] { } });

            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://localhost:12345");
                subSocket.Subscribe("ServerInfo");

                EnqueueServerInfo();
                EnqueueServerInfo();

                string topic = subSocket.ReceiveFrameString();
                string serverInfo = subSocket.ReceiveFrameString();
                string topic2 = subSocket.ReceiveFrameString();
                string serverInfo2 = subSocket.ReceiveFrameString();
            }

            MethodInfo onStop = typeof(PubSubServiceTask)
               .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }

        private void EnqueueServerInfo()
        {
            Enqueue(new PublishModel()
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
            });
        }

        
        [TestMethod]
        public void PublisherTask_HostInfoTest()
        {
            var service = new PubSubServiceTask(this);

            MethodInfo onStart = typeof(PubSubServiceTask)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onStart.Invoke(service, new object[] { new string[] { } });

            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect("tcp://localhost:12345");
                subSocket.Subscribe("HostInfo");

                EnqueueHostInfo(true);
                EnqueueHostInfo(false);

                Assert.AreEqual(subSocket.ReceiveFrameString(), "HostInfo");

                HostInfoModel hostInfo = JsonConvert.DeserializeObject<HostInfoModel>(subSocket.ReceiveFrameString());
                Assert.AreEqual(hostInfo.HostName, Dns.GetHostName());
                Assert.IsTrue(hostInfo.Monitoring);

                Assert.AreEqual(subSocket.ReceiveFrameString(), "HostInfo");

                HostInfoModel hostInfo2 = JsonConvert.DeserializeObject<HostInfoModel>(subSocket.ReceiveFrameString());
                Assert.AreEqual(hostInfo2.HostName, Dns.GetHostName());
                Assert.IsFalse(hostInfo2.Monitoring);
            }

            MethodInfo onStop = typeof(PubSubServiceTask)
               .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }

        private void EnqueueHostInfo(bool monitoring)
        {
            Enqueue(new PublishModel()
            {
                Topic = "HostInfo",
                Data = JObject.FromObject(new HostInfoModel()
                {
                    HostName = Dns.GetHostName(),
                    Monitoring = monitoring
                })
            });
        }

        public PublishModel Dequeue()
        {

            lock (publishQueue)
            {
                if (publishQueue.Count > 0)
                    return publishQueue.Dequeue();
                return null;
            }
        }

        public void Enqueue(PublishModel item)
        {
            lock (publishQueue)
                publishQueue.Enqueue(item);
        }
    }
}
