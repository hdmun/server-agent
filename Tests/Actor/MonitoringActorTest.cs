using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerAgent.Actor;
using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.Data.Entity;
using ServerAgent.Data.Provider;
using System;
using System.Threading.Tasks;

namespace Tests.Actor
{
    [TestClass]
    public class MonitoringActorTest
    {
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

            public TaskCompletionSource<bool> Tcs { get; set; }

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
                catch (Exception ex)
                {
                    Tcs.SetException(ex);
                }
            }
        }

        [TestMethod]
        public void TestMonitoringActor_Tell()
        {
            using (var actorSys = ActorSystem.Create("TestActorSystem"))
            {
                var actorMock = new MonitoringActorMock();
                var monitoringActor = actorSys.ActorOf(actorMock, "MonitoringActorMock");

                actorMock.Tcs = new TaskCompletionSource<bool>();
                monitoringActor.Tell(new AliveCheckMessage(), null);
                Assert.IsTrue(actorMock.Tcs.Task.Result);

                actorMock.Tcs = new TaskCompletionSource<bool>();
                monitoringActor.Tell(new object(), null);
                Assert.ThrowsException<AggregateException>(() =>
                {
                    actorMock.Tcs.Task.Wait();
                });
            }
        }
    }
}
