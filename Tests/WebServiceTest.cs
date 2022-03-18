using Microsoft.VisualStudio.TestTools.UnitTesting;
using server_agent.Web;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Text;

namespace Tests
{
    [TestClass]
    public class WebServiceTest : IWebServiceContext
    {
        public bool Monitoring { get; set; } = false;

        public void OnServerKill()
        {
        }

        [TestMethod]
        public void MonitoringOnOff_Test()
        {
            var service = new WebService(this);

            MethodInfo onStart = typeof(WebService)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onStart.Invoke(service, new object[] { new string[] { } });

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:80/");
                client.DefaultRequestHeaders
                      .Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));// ACCEPT 헤더

                var responseOn = client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Put, "/server/monitoring")
                    {
                        Content = new StringContent("{\"on\":\"true\"}", Encoding.UTF8, "application/json")
                    }).GetAwaiter().GetResult();
                Assert.AreEqual(responseOn.StatusCode, HttpStatusCode.OK);
                Assert.AreEqual(Monitoring, true);

                var responseOff = client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Put, "/server/monitoring")
                    {
                        Content = new StringContent("{\"on\":\"false\"}", Encoding.UTF8, "application/json")
                    }).GetAwaiter().GetResult();
                Assert.AreEqual(responseOff.StatusCode, HttpStatusCode.OK);
                Assert.AreEqual(Monitoring, false);

                var responseEmtpy = client.SendAsync(
                    new HttpRequestMessage(HttpMethod.Put, "/server/monitoring")
                    {
                        Content = new StringContent("{}", Encoding.UTF8, "application/json")
                    }).GetAwaiter().GetResult();
                Assert.AreEqual(responseEmtpy.StatusCode, HttpStatusCode.InternalServerError);
                Assert.AreEqual(Monitoring, false);
            }

            MethodInfo onStop = typeof(WebService)
                .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }

        [Ignore]
        [TestMethod]
        public void ProcessKill_Test()
        {
            var service = new WebService(this);

            MethodInfo onStart = typeof(WebService)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onStart.Invoke(service, new object[] { new string[] { } });

            using (HttpClient client = new HttpClient())
            {
                var response = client.PutAsync("http://localhost:80/server/process/kill", null)
                    .GetAwaiter()
                    .GetResult();
                Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);

                response = client.PutAsync("http://localhost:80/server/monitoring/unknown", null)
                    .GetAwaiter()
                    .GetResult();
                Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
            }

            MethodInfo onStop = typeof(WebService)
                .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }
    }
}
