using ServerAgent.PubSub.Model;

namespace ServerAgent.PubSub
{
    public interface IPubSubQueue
    {
        PublishModel Dequeue();

        void Enqueue(PublishModel item);
    }
}
