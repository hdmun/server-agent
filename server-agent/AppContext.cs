using NetMQ;
using Newtonsoft.Json;
using server_agent.Interactor;
using server_agent.Monitoring.Data;
using server_agent.Monitoring.Data.Provider;
using server_agent.Network;
using server_agent.Network.Model;
using System.Collections.Generic;

namespace server_agent
{
    public class AppContext : IContext, IRequestHandler
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

        public void OnRequest(IOutgoingSocket socket, RequestModel reqModel)
        {
            switch (reqModel.Type)
            {
                case Protocol.Monitoring:
                    var model = reqModel.Data.ToObject<MonitoringModel>();

                    Monitoring = model.On;

                    socket.SendFrame(JsonConvert.SerializeObject(new ResponseModel()
                    {
                        Success = true,
                        Message = "Success",
                        Request = JsonConvert.SerializeObject(reqModel)
                    }));
                    break;
                default:
                    socket.SendFrame(JsonConvert.SerializeObject(new ResponseModel()
                    {
                        Success = false,
                        Message = "Invalid Protocol Message",
                        Request = JsonConvert.SerializeObject(reqModel)
                    }));
                    break;
            }
        }
    }
}
