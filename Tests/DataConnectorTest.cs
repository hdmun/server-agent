using Microsoft.VisualStudio.TestTools.UnitTesting;
using ServerAgent.Data;
using ServerAgent.Data.Provider;
using ServerAgent.Monitoring.Interactor;
using System.Collections.Generic;

namespace Tests
{
    [TestClass]
    public class DataConnectorTest
    {
        public void OnMonitoring()
        {
        }

        [TestMethod]
        public void TestJsonProvider()
        {
            DataConnector dataConnector = new DataConnector(DataProviderFactory.Create("json"));
            Assert.IsTrue(dataConnector.Open());

            var detectTime = dataConnector.DetectTime();
            var Processes = new List<ServerProcess>();
            foreach (var serverInfo in dataConnector.ServerInfo())
            {
                var timeChekr = TimeCheckerFactory.Create(detectTime);
                Assert.AreNotEqual(timeChekr, null);
                Processes.Add(new ServerProcess(serverInfo, timeChekr));
            }

            Assert.AreEqual(Processes.Count, 2);
            Assert.AreNotEqual(detectTime, null);
        }


        [TestMethod]
        public void TestSqlProvider()
        {
            DataConnector dataConnector = new DataConnector(DataProviderFactory.Create("sql"));
            Assert.IsTrue(dataConnector.Open());

            var detectTime = dataConnector.DetectTime();
            var Processes = new List<ServerProcess>();
            foreach (var serverInfo in dataConnector.ServerInfo())
            {
                var timeChekr = TimeCheckerFactory.Create(detectTime);
                Assert.AreNotEqual(timeChekr, null);
                Processes.Add(new ServerProcess(serverInfo, timeChekr));
            }

            Assert.AreEqual(Processes.Count, 2);
            Assert.AreNotEqual(detectTime, null);
        }
    }
}
