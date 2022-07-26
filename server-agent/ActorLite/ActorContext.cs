namespace ServerAgent.ActorLite
{
    public class ActorContext
    {
        private readonly ActorSystem _system;
        private readonly IActorRef _parent;

        public ActorSystem System { get => _system; }
        public IActorRef Sender { get; set; }
        public IActorRef Parent { get => _parent; }

        public ActorContext(ActorSystem system, IActorRef parent)
        {
            _system = system;
            _parent = parent;
        }

        public IActorRef ActorOf(ActorRefBase actor, string name, IActorRef parent = null)
        {
            return _system.ActorOf(actor, name, parent);
        }
    }
}
