using server_agent.PubSub.Model;

namespace server_agent.PubSub
{
    public interface IPubSubQueue
    {
        PublishModel Dequeue();

        void Enqueue(PublishModel item);
    }
}
