using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ServerAgent.Actor;
using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.Data.Entity;
using ServerAgent.Data.Model;
using ServerAgent.Data.Provider;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Actor
{
    [TestClass]
    public class HttpServerActorTest
    {
        internal class DataProviderMock : IDataProvider
        {
            public MonitoringConfig FindMonitoringConfig(string hostName)
            {
                return new MonitoringConfig();
            }

            public List<ServerProcess> FindProcesses(string hostName)
                => new List<ServerProcess>();
        }

        internal class MonitoringActorMock : MonitoringActor
        {
            public MonitoringActorMock()
                : base(new DataProviderMock())
            {
            }

            public TaskCompletionSource<bool> Tcs { private get; set; }

            public bool WaitResult() => Tcs.Task.Result;

            protected override void OnStart()
            {
            }

            protected override void OnReceive(object message)
            {
                try
                {
                    base.OnReceive(message);
                    Tcs.SetResult(true);
                }
                catch (Exception)
                {
                    Tcs.SetResult(false);
                }
            }
        }

        internal class HttpServerActorMock : HttpServerActor
        {
            public HttpServerActorMock(IActorRef monitoringActor)
                : base("http://localhost:8080/", monitoringActor)
            {
            }
        }

        private ActorSystem _actorSystem = ActorSystem.Create("TestActorSystem");

        [TestMethod]
        public void HttpServerActor_Test()
        {
            var monitoringActor = _actorSystem.ActorOf(new MonitoringActorMock(), "MonitoringActorMock");
            _actorSystem.ActorOf(new HttpServerActorMock(monitoringActor), "HttpServerActorMock");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:8080/");
                client.DefaultRequestHeaders.Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));  // ACCEPT 헤더

                var requestData = JsonConvert.SerializeObject(new ServerMonitoringRequest()
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
