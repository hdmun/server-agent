using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ServerAgent.Actor;
using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.Data.Entity;
using ServerAgent.Data.Provider;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Tests.Actor
{
    [TestClass]
    public class HttpServerActorTest
    {
        private static readonly string _bindUrl = "http://localhost:8080/";

        internal class DataProviderMock : IDataProvider
        {
            public MonitoringConfig FindMonitoringConfig(string hostName)
            {
                return new MonitoringConfig();
            }

            public ServerProcess[] FindProcesses(string hostName)
                => new ServerProcess[] { };
        }

        internal class MonitoringActorMock : MonitoringActor
        {
            public MonitoringActorMock()
                : base(new DataProviderMock())
            {
            }
        }

        internal class HttpServerActorMock : HttpServerActor
        {
            public HttpServerActorMock(IActorRef monitoringActor)
                : base(_bindUrl, monitoringActor)
            {
            }
        }

        [TestMethod]
        public void UpdateMonitoring_Test()
        {
            using (var actorSys = ActorSystem.Create("TestActorSystem"))
            {
                var monitoringActor = actorSys.ActorOf(new MonitoringActorMock(), "MonitoringActorMock");
                actorSys.ActorOf(new HttpServerActorMock(monitoringActor), "HttpServerActorMock");

                var json = JsonConvert.SerializeObject(new MonitoringMessage()
                {
                    HostName = Dns.GetHostName(),
                    On = true
                });

                var obj = RequestConent<MonitoringMessage>("PATCH", "/monitoring", json, HttpStatusCode.Created);
                Assert.IsTrue(obj.On);
            }
        }

        [TestMethod]
        public void GetProcessState_Test()
        {
            using (var actorSys = ActorSystem.Create("TestActorSystem"))
            {
                var monitoringActor = actorSys.ActorOf(new MonitoringActorMock(), "MonitoringActorMock");
                actorSys.ActorOf(new HttpServerActorMock(monitoringActor), "HttpServerActorMock");

                var obj = RequestNoContent<ProcessStateResponse>("GET", "/process/TestServer1", HttpStatusCode.OK);
                Assert.AreEqual(obj.ThreadId, 0u);
                Assert.AreEqual(obj.ReceiveTime, "");
                Assert.AreEqual(obj.ProcessingTime, 0u);
            }
        }

        [TestMethod]
        public void DeleteProcess_Test()
        {
            using (var actorSys = ActorSystem.Create("TestActorSystem"))
            {
                var monitoringActor = actorSys.ActorOf(new MonitoringActorMock(), "MonitoringActorMock");
                actorSys.ActorOf(new HttpServerActorMock(monitoringActor), "HttpServerActorMock");

                var obj = RequestNoContent<ServerKillResponse>("DELETE", "/process/TestServer1/close", HttpStatusCode.OK);
                Assert.IsNull(obj.Servers);
            }
        }

        private T RequestConent<T>(string method, string requestUrl, string json, HttpStatusCode statusCode)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_bindUrl);
                client.DefaultRequestHeaders
                    .Accept
                    .Add(new MediaTypeWithQualityHeaderValue("application/json"));  // ACCEPT 헤더

                var requestMessage = new HttpRequestMessage(new HttpMethod(method), requestUrl)
                {
                    Content = new StringContent(json, Encoding.UTF8, "application/json")
                };
                var response = client.SendAsync(requestMessage).Result;
                Assert.AreEqual(response.StatusCode, statusCode);

                var responseData = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(responseData);
            }
        }

        private T RequestNoContent<T>(string method, string requestUrl, HttpStatusCode statusCode)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_bindUrl);

                var requestMessage = new HttpRequestMessage(new HttpMethod(method), requestUrl);
                var response = client.SendAsync(requestMessage).Result;
                Assert.AreEqual(response.StatusCode, statusCode);

                var responseData = response.Content.ReadAsStringAsync().Result;
                return JsonConvert.DeserializeObject<T>(responseData);
            }
        }
    }
}
