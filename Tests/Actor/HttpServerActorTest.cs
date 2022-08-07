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
        public void HttpServerActor_Test()
        {
            ActorSystem actorSystem = ActorSystem.Create("TestActorSystem");
            var monitoringActor = actorSystem.ActorOf(new MonitoringActorMock(), "MonitoringActorMock");
            actorSystem.ActorOf(new HttpServerActorMock(monitoringActor), "HttpServerActorMock");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_bindUrl);
                client.DefaultRequestHeaders.Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));  // ACCEPT 헤더

                var requestData = JsonConvert.SerializeObject(new MonitoringMessage()
                {
                    HostName = Dns.GetHostName(),
                    On = true
                });
                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), "/server/monitoring")
                {
                    Content = new StringContent(requestData, Encoding.UTF8, "application/json")
                };
                var response = client.SendAsync(requestMessage).Result;
                Assert.AreEqual(response.StatusCode, HttpStatusCode.Created);

                var responseData = response.Content.ReadAsStringAsync().Result;
                var responseObj = JsonConvert.DeserializeObject<MonitoringMessage>(responseData);
                Assert.IsTrue(responseObj.On);
            }
        }
    }
}
