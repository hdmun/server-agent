using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.ActorLite.Http;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;

namespace Tests.ActorLite
{
    [TestClass]
    public class HttpListenActorTest
    {
        private static readonly string _bindUrl = "http://localhost:8080/";

        internal class HttpListenActorMock : HttpListenActor
        {
            public HttpListenActorMock()
                : base(_bindUrl)
            {
            }
        }

        [TestMethod]
        public void HttpReceive_Test()
        {
            ActorSystem actorSystem = ActorSystem.Create("TestActorSystem");
            actorSystem.ActorOf(new HttpListenActorMock(), "HttpListenActorMock");

            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_bindUrl);
                client.DefaultRequestHeaders.Accept
                      .Add(new MediaTypeWithQualityHeaderValue("application/json"));  // ACCEPT 헤더

                var emptyJsonText = new StringContent("{}", Encoding.UTF8, "application/json");
                var requestMessage = new HttpRequestMessage(new HttpMethod("PATCH"), "/monitoring")
                {
                    Content = emptyJsonText
                };
                var response = client.SendAsync(requestMessage).Result;
                Assert.AreEqual(response.StatusCode, HttpStatusCode.NotFound);

                var responseData = response.Content.ReadAsStringAsync().Result;
                Assert.AreEqual(responseData, "");
            }

            actorSystem.Dispose();
        }
    }
}
