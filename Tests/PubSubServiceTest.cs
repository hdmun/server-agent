using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServerAgent.Messaging;
using ServerAgent.Messaging.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace Tests
{
    [TestClass]
    public class PubSubServiceTest : IMessagingQueue
    {
        private Queue<PublishModel> publishQueue = new Queue<PublishModel>();

        [TestMethod]
        public void PublisherTask_ServerInfoTest()
        {
            var serviceTask = new MessagingServiceTask(this);
            serviceTask.OnStart();

            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect(ConfigurationManager.AppSettings["PublisherAddr"]);
                subSocket.Subscribe("ServerInfo");

                EnqueueServerInfo();
                EnqueueServerInfo();

                string topic = subSocket.ReceiveFrameString();
                string serverInfo = subSocket.ReceiveFrameString();
                string topic2 = subSocket.ReceiveFrameString();
                string serverInfo2 = subSocket.ReceiveFrameString();
            }

            serviceTask.OnStop();
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
            var serviceTask = new MessagingServiceTask(this);
            serviceTask.OnStart();

            using (var subSocket = new SubscriberSocket())
            {
                subSocket.Options.ReceiveHighWatermark = 1000;
                subSocket.Connect(ConfigurationManager.AppSettings["PublisherAddr"]);
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

            serviceTask.OnStop();
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
