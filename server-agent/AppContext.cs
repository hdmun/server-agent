﻿using log4net;
using Newtonsoft.Json.Linq;
using server_agent.Data;
using server_agent.Data.Provider;
using server_agent.Monitoring;
using server_agent.Monitoring.Interactor;
using server_agent.PubSub;
using server_agent.PubSub.Model;
using server_agent.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Net;

namespace server_agent
{
    public class AppContext : IMonitoringContext, IPubSubQueue, IWebServiceContext
    {
        private readonly ILog logger;

        private bool monitoring;
        private DataConnector dataConnector;

        private Queue<PublishModel> publishQueue;

        public AppContext()
        {
            logger = LogManager.GetLogger(typeof(AppContext));

            monitoring = false;
            Processes = null;

            var providerName = ConfigurationManager.AppSettings["DataProvider"];
            dataConnector = new DataConnector(DataProviderFactory.Create(providerName));

            publishQueue = new Queue<PublishModel>();
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
                {
                    logger.Info($"change monitoring flag, before: {monitoring}, after: {value}");
                    monitoring = value;
                }
            }
        }

        public List<ServerProcess> Processes { get; private set; }

        public void OnStart()
        {
            logger.Info("start context");

            if (!dataConnector.Open())
            {
                logger.Error("failed to open `DataConnector`");
                return;  // throw exception?
            }

            var detectTime = dataConnector.DetectTime();
            Processes = new List<ServerProcess>();
            foreach (var serverInfo in dataConnector.ServerInfo())
            {
                Processes.Add(new ServerProcess(serverInfo, detectTime));
            }

            logger.Info($"load process list, process count: {Processes.Count}");
        }

        public void OnMonitoring()
        {
            foreach (var process in Processes)
            {
                var processInfo = process.ProcessInfo;

                Enqueue(new PublishModel()
                {
                    Topic = "ServerInfo",
                    Data = JObject.FromObject(new ServerInfoModel()
                    {
                        HostName = Dns.GetHostName(),
                         ProcessName = process.FilePath,
                         ServerName = process.ServerName,
                         ProcessingTime = processInfo.ProcessingTime,
                         ThreadId = processInfo.ThreadId,
                         LastReceiveTime = processInfo.LastReceiveTime
                    })
                });
            }
        }

        public void Enqueue(PublishModel item)
        {
            lock (publishQueue)
            {
                publishQueue.Enqueue(item);
            }
        }

        public PublishModel Dequeue()
        {
            lock (publishQueue)
            {
                if (publishQueue.Count > 0)
                    return publishQueue.Dequeue();
                return null;
            }
        }

        public void OnServerKill()
        {
            foreach (var process in Processes)
            {
                process.Close();
            }
        }
    }
}
