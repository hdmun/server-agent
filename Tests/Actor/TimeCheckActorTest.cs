using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerAgent.Actor;
using ServerAgent.ActorLite;
using ServerAgent.Data.Entity;
using System;
using System.Net;

namespace Tests.Actor
{
    [TestClass]
    internal class TimeCheckActorTest
    {
        [TestMethod]
        public void TimeCheckActor_Factory_Test()
        {
            var config = new MonitoringConfig()
            {
                HostName = Dns.GetHostName(),
                DeadlockMin = 1,
                StoppedMin = 1,
                Checker = ""
            };

            Assert.ThrowsException<Exception>(() =>
            {
                TimeCheckActorFactory.Create(config);
            });

            config.Checker = "datetime";
            var dateTimeCheckActor = TimeCheckActorFactory.Create(config);
            Assert.AreEqual(dateTimeCheckActor.GetType(), typeof(DateTimeCheckActor));

            config.Checker = "timegettime";
            var tickTimeCheckActor = TimeCheckActorFactory.Create(config);
            Assert.AreEqual(tickTimeCheckActor.GetType(), typeof(DateTimeCheckActor));
        }
    }
}
