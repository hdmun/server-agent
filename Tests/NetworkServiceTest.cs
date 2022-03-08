using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetMQ;
using NetMQ.Sockets;
using Newtonsoft.Json;
using server_agent.Network;
using server_agent.Network.Model;
using System.Reflection;

namespace Tests
{
    [TestClass]
    public class NetworkServiceTest : IRequestHandler
    {
        [TestMethod]
        public void ResponseTask_Test()
        {
            var service = new NetworkService(this);

            MethodInfo onStart = typeof(NetworkService)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            onStart.Invoke(service, new object[] { new string[] { } });

            using (var client = new RequestSocket())
            {
                client.Connect("tcp://localhost:5555");

                string reqMonitoring = "{\"type\":\"Monitoring\",\"data\":{\"on\":true}}";
                client.SendFrame(reqMonitoring);

                ResponseModel res = JsonConvert.DeserializeObject<ResponseModel>(client.ReceiveFrameString());
                Assert.AreEqual(res.Success, true);
                Assert.AreEqual(res.Message, "Success");
                Assert.AreEqual(res.Request, reqMonitoring);
            }

            MethodInfo onStop = typeof(NetworkService)
               .GetMethod("OnStop", BindingFlags.Instance | BindingFlags.NonPublic);
            onStop.Invoke(service, new object[] { });
        }

        public void OnRequest(IOutgoingSocket socket, RequestModel reqModel)
        {
            switch (reqModel.Type)
            {
                case Protocol.Monitoring:
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
