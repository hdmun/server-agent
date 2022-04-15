using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using ServerAgent.Web;
using ServerAgent.Web.Model;
using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class WebServiceTest : IWebServiceContext
    {
        public bool Monitoring { get; set; } = false;

        ServerKillResponseModel[] IWebServiceContext.OnServerKill()
        {
            return new ServerKillResponseModel[] { };
        }

        public ServerKillResponseModel[] OnServerClose()
        {
            return new ServerKillResponseModel[] { };
        }

        public ServerKillResponseModel OnServerKill(string serverName)
        {
            return new ServerKillResponseModel();
        }

        public ServerKillResponseModel OnServerClose(string serverName)
        {
            return new ServerKillResponseModel();
        }

        [TestMethod]
        public void MonitoringOn_Test()
        {
            var serviceTask = new WebServiceTask(this);
            serviceTask.OnStart();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri($"{ConfigurationManager.AppSettings["HttpUrl"]}");
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));// ACCEPT 헤더

                ServerMonitoringModel reqOn = new ServerMonitoringModel()
                {
                    HostName = Dns.GetHostName(),
                    On = true
                };
                var responseOn = client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Put, "/server/monitoring")
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(reqOn), Encoding.UTF8, "application/json")
                    }).GetAwaiter().GetResult();
                Assert.AreEqual(responseOn.StatusCode, HttpStatusCode.Created);
                Assert.IsTrue(Monitoring);


                var resStringOn = responseOn.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var resModelOn = JsonConvert.DeserializeObject<ServerMonitoringModel>(resStringOn);
                Assert.AreEqual(resModelOn.HostName, Dns.GetHostName());
                Assert.IsTrue(resModelOn.On);

                ServerMonitoringModel reqOff = new ServerMonitoringModel()
                {
                    HostName = Dns.GetHostName(),
                    On = false
                };
                var responseOff = client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Put, "/server/monitoring")
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(reqOff), Encoding.UTF8, "application/json")
                    }).GetAwaiter().GetResult();
                Assert.AreEqual(responseOff.StatusCode, HttpStatusCode.Created);
                Assert.IsFalse(Monitoring);

                var resStringOff = responseOff.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                var resModelOff = JsonConvert.DeserializeObject<ServerMonitoringModel>(resStringOff);
                Assert.AreEqual(resModelOff.HostName, Dns.GetHostName());
                Assert.IsFalse(resModelOff.On);
            }

            serviceTask.OnStop();
        }

        [TestMethod]
        public void MonitoringInvalid_Test()
        {
            var serviceTask = new WebServiceTask(this);
            serviceTask.OnStart();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(ConfigurationManager.AppSettings["HttpUrl"]);
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));// ACCEPT 헤더

                ServerMonitoringModel reqInvalidHost = new ServerMonitoringModel()
                {
                    HostName = "Invalid HostName",
                    On = false
                };
                var responseError = client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Put, "/server/monitoring")
                    {
                        Content = new StringContent(JsonConvert.SerializeObject(reqInvalidHost), Encoding.UTF8, "application/json")
                    }).GetAwaiter().GetResult();
                Assert.AreEqual(responseError.StatusCode, HttpStatusCode.BadRequest);


                var responseEmtpy = client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Put, "/server/monitoring")
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json")
                    }).GetAwaiter().GetResult();
                Assert.AreEqual(responseEmtpy.StatusCode, HttpStatusCode.InternalServerError);
                Assert.AreEqual(Monitoring, false);
            }

            serviceTask.OnStop();
        }

        [Ignore]
        [TestMethod]
        public async Task ProcessKill_Test()
        {
            var serviceTask = new WebServiceTask(this);
            serviceTask.OnStart();

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));// ACCEPT 헤더

                var reqServerKill = new ServerKillRequestModel()
                {
                    KillCommand = "killAll"
                };
                var reponseKillAll = await client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Put, "/server/process/kill")
                    {
                        Content = new StringContent(
                            JsonConvert.SerializeObject(reqServerKill),
                            Encoding.UTF8,
                            "application/json")
                    });
                var resStringKillAll = await reponseKillAll.Content.ReadAsStringAsync();
                var resModelKillAll = JsonConvert.DeserializeObject<ServerKillResponseModel[]>(resStringKillAll);
                Assert.AreEqual(reponseKillAll.StatusCode, HttpStatusCode.OK);
                Assert.AreEqual(resModelKillAll.Length, 0);
            }

            serviceTask.OnStop();
        }
    }
}
