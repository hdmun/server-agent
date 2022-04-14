using log4net;
using Newtonsoft.Json.Linq;
using ServerAgent.Data;
using ServerAgent.Data.Provider;
using ServerAgent.Monitoring;
using ServerAgent.Monitoring.Interactor;
using ServerAgent.PubSub;
using ServerAgent.PubSub.Model;
using ServerAgent.Web;
using ServerAgent.Web.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using Topshelf;

namespace ServerAgent
{
    public class AgentService : IMonitoringContext, IPubSubQueue, IWebServiceContext, ServiceControl
    {
        private readonly ILog logger;
        private readonly IServiceTask[] serviceTasks;

        private bool monitoring;
        private DataConnector dataConnector;

        private Queue<PublishModel> publishQueue;

        public AgentService()
        {
            logger = LogManager.GetLogger(typeof(AgentService));

            serviceTasks = new IServiceTask[] {
                new MonitoringServiceTask(this),
                new PubSubServiceTask(this),
                new WebServiceTask(this)
            };

            monitoring = false;
            Processes = null;

            var providerName = ConfigurationManager.AppSettings["DataProvider"];
            dataConnector = new DataConnector(DataProviderFactory.Create(providerName));

            publishQueue = new Queue<PublishModel>();
        }

        bool ServiceControl.Start(HostControl hostControl)
        {
            logger.Info("start agent service");

            if (!dataConnector.Open())
            {
                logger.Error("failed to open `DataConnector`");
                throw new Exception("failed to Open, `dataConnector` in AppContext.OnStart");
            }

            var detectTime = dataConnector.DetectTime();
            Processes = new List<ServerProcess>();
            foreach (var serverInfo in dataConnector.ServerInfo())
            {
                Processes.Add(new ServerProcess(serverInfo, TimeCheckerFactory.Create(detectTime)));
            }

            logger.Info($"load process list, process count: {Processes.Count}");

            foreach (var task in serviceTasks)
            {
                task.OnStart();
            }

            return true;
        }

        bool ServiceControl.Stop(HostControl hostControl)
        {
            foreach (var task in serviceTasks)
            {
                task.OnStop();
            }

            logger.Info("stop agent service");
            return true;
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

        public void OnMonitoring()
        {
            Enqueue(new PublishModel()
            {
                Topic = "HostInfo",
                Data = JObject.FromObject(new HostInfoModel()
                {
                    HostName = Dns.GetHostName(),
                    Monitoring = Monitoring
                })
            });

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
                         IsAlive = !process.IsDead,
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

        public ServerKillResponseModel[] OnServerKill()
        {
            var response = new List<ServerKillResponseModel>();
            foreach (var process in Processes)
            {
                process.Kill();
                response.Add(new ServerKillResponseModel()
                {
                    ServerName = process.ServerName,
                    ExitCode = process.ExitCode,
                    Close = process.IsDead
                });
            }

            return response.ToArray();
        }

        public ServerKillResponseModel[] OnServerClose()
        {
            var response = new List<ServerKillResponseModel>();
            foreach (var process in Processes)
            {
                bool closed = process.Close();
                response.Add(new ServerKillResponseModel()
                {
                    ServerName = process.ServerName,
                    ExitCode = process.ExitCode,
                    Close = closed
                });
            }

            return response.ToArray();
        }

        public ServerKillResponseModel OnServerKill(string serverName)
        {
            foreach (var process in Processes)
            {
                if (process.ServerName == serverName)
                {
                    process.Kill();
                    return new ServerKillResponseModel()
                    {
                        ServerName = process.ServerName,
                        ExitCode = process.ExitCode,
                        Close = process.IsDead
                    };
                }
            }
            return null;
        }

        public ServerKillResponseModel OnServerClose(string serverName)
        {
            foreach (var process in Processes)
            {
                if (process.ServerName == serverName)
                {
                    bool closed = process.Close();
                    return new ServerKillResponseModel()
                    {
                        ServerName = process.ServerName,
                        ExitCode = process.ExitCode,
                        Close = closed
                    };
                }
            }
            return null;
        }
    }
}
