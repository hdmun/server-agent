using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using server_agent.Network.Model;
using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace server_agent.Network
{
    public class NetworkService : ServiceBase
    {
        private readonly IRequestHandler handler;
        private Task responseTask;
        private bool isRunning;

        public NetworkService(IRequestHandler handler)
        {
            this.handler = handler;
            responseTask = null;
            isRunning = false;
        }

        protected override void OnStart(string[] args)
        {
            Debug.WriteLine("NetworkService.OnStart");

            isRunning = true;
            responseTask = Task.Run(() =>
            {
                ResponseTask();
            });
        }

        protected override void OnStop()
        {
            Debug.WriteLine("NetworkService.OnStop");

            isRunning = false;
            responseTask.Wait();
        }

        private void ResponseTask()
        {
            using (var server = new ResponseSocket())
            {
                server.Bind("tcp://*:5555");

                while (isRunning)
                {
                    string reqJson;
                    if (!server.TryReceiveFrameString(out reqJson))
                    {
                        Task.Delay(500).Wait();
                        continue;
                    }

                    try
                    {
                        var reqModel = JsonConvert.DeserializeObject<RequestModel>(reqJson);
                        handler.OnRequest(server, reqModel);
                    }
                    catch (Exception ex)
                    {
                        ResponseModel response = new ResponseModel()
                        {
                            Success = false,
                            Message = ex.Message,
                            Request = reqJson
                        };
                        server.SendFrame(JsonConvert.SerializeObject(response));
                    }
                }
            }
        }
    }
}
