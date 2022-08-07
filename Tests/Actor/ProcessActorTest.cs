using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerAgent.Actor;
using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.Data.Entity;
using System.Net;

namespace Tests.Actor
{
    [TestClass]
    public class ProcessActorTest
    {
        internal class ProcessActorMock : ProcessActor
        {
            public ProcessActorMock(ServerProcess serverProcess, MonitoringConfig config)
                : base(serverProcess, config)
            {
            }
        }

        [TestMethod]
        private void AliveCheckMessage_Test()
        {
            var serverProcessEntity = new ServerProcess()
            {
                HostName = Dns.GetHostName(),
                ServerName = "ServerTest",
                BinaryPath = ""
            };
            var config = new MonitoringConfig()
            {
                DeadlockMin = 1,
                StoppedMin = 2,
                Checker = "datetime"
            };

            ActorSystem actorSystem = ActorSystem.Create("TestActorSystem");
            var processActor = actorSystem.ActorOf(
                new ProcessActor(serverProcessEntity, config), "ProcessActorTset");

            processActor.Tell(new AliveCheckMessage());
        }

        [TestMethod]
        private void ProcessKillMessage_Test()
        {
            var serverProcessEntity = new ServerProcess()
            {
                HostName = Dns.GetHostName(),
                ServerName = "ServerTest",
                BinaryPath = ""
            };
            var config = new MonitoringConfig()
            {
                DeadlockMin = 1,
                StoppedMin = 2,
                Checker = "datetime"
            };
            ActorSystem actorSystem = ActorSystem.Create("TestActorSystem");
            var processActor = actorSystem.ActorOf(
                new ProcessActor(serverProcessEntity, config), "ProcessActorTset");

            processActor.Tell(new ProcessKillRequest());
        }

        [TestMethod]
        private void WorkerThreadMessage_Test()
        {
            var serverProcessEntity = new ServerProcess()
            {
                HostName = Dns.GetHostName(),
                ServerName = "ServerTest",
                BinaryPath = ""
            };
            var config = new MonitoringConfig()
            {
                DeadlockMin = 1,
                StoppedMin = 2,
                Checker = "datetime"
            };
            ActorSystem actorSystem = ActorSystem.Create("TestActorSystem");
            var processActor = actorSystem.ActorOf(
                new ProcessActor(serverProcessEntity, config), "ProcessActorTset");

            processActor.Tell(new WorkerThreadMessage());
        }

        [TestMethod]
        private void ProcessStoppedMessage_Test()
        {
            var serverProcessEntity = new ServerProcess()
            {
                HostName = Dns.GetHostName(),
                ServerName = "ServerTest",
                BinaryPath = ""
            };
            var config = new MonitoringConfig()
            {
                DeadlockMin = 1,
                StoppedMin = 2,
                Checker = "datetime"
            };
            ActorSystem actorSystem = ActorSystem.Create("TestActorSystem");
            var processActor = actorSystem.ActorOf(
                new ProcessActor(serverProcessEntity, config), "ProcessActorTset");

            processActor.Tell(new ProcessStoppedMessage());
        }
    }
}
