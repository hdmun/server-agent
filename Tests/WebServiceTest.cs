using Microsoft.VisualStudio.TestTools.UnitTesting;
using server_agent.Web;
using System.Net;
using System.Net.Http;
using System.Reflection;

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
                var response = client.PutAsync("http://localhost:80/server/monitoring/on", null)
                    .GetAwaiter()
                    .GetResult();
                Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
                Assert.AreEqual(Monitoring, true);

                response = client.PutAsync("http://localhost:80/server/monitoring/off", null)
                    .GetAwaiter()
                    .GetResult();
                Assert.AreEqual(response.StatusCode, HttpStatusCode.OK);
                Assert.AreEqual(Monitoring, false);

                response = client.PutAsync("http://localhost:80/server/monitoring", null)
                    .GetAwaiter()
                    .GetResult();
                Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);
                Assert.AreEqual(Monitoring, false);
            }

            MethodInfo onStop = typeof(WebService)
                .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }

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
