using NetMQ;
using server_agent.Network.Model;

namespace server_agent.Network
{
    public interface IRequestHandler
    {
        void OnRequest(IOutgoingSocket socket, RequestModel reqModel);
    }
}
