using System.Threading.Tasks;

namespace ServerAgent.ActorLite
{
    public interface IActorRef
    {
        void Stop();

        void Tell(object message);
    }
}
