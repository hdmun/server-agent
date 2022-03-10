using NetMQ;
using server_agent.Network.Model;
using server_agent.Network.Publish;

namespace server_agent.Network
{
    public interface INetworkHandler
    {
        void OnRequest(IOutgoingSocket socket, RequestModel reqModel);

        PublishModel Dequeue();

        void Enqueue(PublishModel item);
    }
}
