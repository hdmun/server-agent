using Microsoft.VisualStudio.TestTools.UnitTesting;
using server_agent.Data;
using server_agent.Data.Provider;
using server_agent.Monitoring.Interactor;
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
        public void TestJsonConfig()
        {
            DataConnector dataConnector = new DataConnector(DataProviderFactory.Create("json"));
            Assert.IsTrue(dataConnector.Open());

            var detectTime = dataConnector.DetectTime();
            var Processes = new List<ServerProcess>();
            foreach (var serverInfo in dataConnector.ServerInfo())
                Processes.Add(new ServerProcess(serverInfo, detectTime));

            Assert.AreEqual(Processes.Count, 2);
            Assert.AreNotEqual(detectTime, null);
        }
    }
}
