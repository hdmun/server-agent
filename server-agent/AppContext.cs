﻿using server_agent.Data;
using server_agent.Data.Provider;
using server_agent.Interactor;
using System.Collections.Generic;

namespace server_agent
{
    public class AppContext : IContext
    {
        private bool monitoring;
        private DataConnector dataConnector;

        public AppContext()
        {
            monitoring = true;
            Processes = null;
            dataConnector = new DataConnector(DataProviderFactory.Create("json"));
        }

        public bool Monitoring
        {
            get
            {
                lock (this)
                    return monitoring;
            }
            set
            {
                lock (this)
                    monitoring = value;
            }
        }

        public List<ServerProcess> Processes { get; private set; }

        public void OnStart()
        {
            if (!dataConnector.Open())
            {
                return;  // throw exception
            }

            var detectTime = dataConnector.DetectTime();
            Processes = new List<ServerProcess>();
            foreach (var serverInfo in dataConnector.ServerInfo())
            {
                Processes.Add(new ServerProcess(serverInfo, detectTime));
            }
        }
    }
}
