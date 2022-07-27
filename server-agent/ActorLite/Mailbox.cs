namespace ServerAgent.ActorLite
{
    internal class Mailbox
    {
        public object Message { get; private set; }

        public IActorRef Sender { get; private set; }
        public Mailbox(object message, IActorRef sender)
        {
            Message = message;
            Sender = sender;
        }
    }
}
