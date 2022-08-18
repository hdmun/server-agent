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
        internal class HttpListenActorMock : HttpListenActor
        {
            public HttpListenActorMock()
                : base(HttpRequestMock.BindUrl)
            {
            }
        }

        internal class EmptyObject { }

        [TestMethod]
        public void HttpReceive_Test()
        {
            ActorSystem actorSystem = ActorSystem.Create("TestActorSystem");
            actorSystem.ActorOf(new HttpListenActorMock(), "HttpListenActorMock");

            HttpRequestMock.RequestConent<EmptyObject>("PATCH", "/monitoring", "{}", HttpStatusCode.NotFound);

            actorSystem.Dispose();
        }
    }
}
