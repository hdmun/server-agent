using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerAgent.ActorLite;
using System;

namespace Tests.ActorLite
{
    [TestClass]
    public class ActorTimerTest
    {
        internal class ActorMock : ActorRefBase
        {
            protected override void OnReceive(object message)
            {
            }
        }

        [TestMethod]
        public void ActorNull_Test()
        {
            var actorTimer = new ActorTimer(null);

            Assert.ThrowsException<NullReferenceException>(() =>
            {
                actorTimer.Start(new object(), 1);
            });
        }

        [TestMethod]
        public void StartDuplicate_Test()
        {
            var actorTimer = new ActorTimer(new ActorMock());
            actorTimer.Start(new object(), 1);

            Assert.ThrowsException<Exception>(() =>
            {
                actorTimer.Start(new object(), 1);
            });
        }
    }
}
