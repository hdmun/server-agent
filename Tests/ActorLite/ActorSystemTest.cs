using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerAgent.ActorLite;
using System;

namespace Tests.ActorLite
{
    [TestClass]
    public class ActorSystemTest
    {
        internal class ActorMock : ActorRefBase
        {
            protected override void OnReceive(object message)
            {
            }

            protected override void OnStart()
            {
            }
        }

        [TestMethod]
        public void DuplicateAcotr_Test()
        {
            var actorSystem = ActorSystem.Create("TestActorSystem");
            actorSystem.ActorOf(new ActorMock(), "ActorBaseMock");

            Assert.ThrowsException<Exception>(() =>
            {
                actorSystem.ActorOf(new ActorMock(), "ActorBaseMock");
            });

            actorSystem.Dispose();
        }

        [TestMethod]
        public void Dispose_Test()
        {
            var actorSystem = ActorSystem.Create("TestActorSystem");
            actorSystem.Dispose();
        }
    }
}
