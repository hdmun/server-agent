using NetMQ;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using server_agent.Monitoring.Data;
using server_agent.Monitoring.Data.Provider;
using server_agent.Monitoring.Interactor;
using server_agent.PubSub;
using server_agent.PubSub.Model;
using System.Collections.Generic;

namespace server_agent
{
    public class AppContext : IContext, IPubSubQueue
    {
        private bool monitoring;
        private DataConnector dataConnector;

        private Queue<PublishModel> publishQueue;

        public AppContext()
        {
            monitoring = true;
            Processes = null;
            dataConnector = new DataConnector(DataProviderFactory.Create("json"));

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
    }
}
