using ServerAgent.Messaging.Model;

namespace ServerAgent.Messaging
{
    public interface IMessagingQueue
    {
        PublishModel Dequeue();

        void Enqueue(PublishModel item);
    }
}
