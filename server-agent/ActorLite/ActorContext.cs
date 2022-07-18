namespace ServerAgent.ActorLite
{
    public class ActorContext
    {
        private readonly ActorSystem _system;

        public ActorSystem System { get => _system; }
        public IActorRef Sender { get; set; }

        public ActorContext(ActorSystem system)
        {
            _system = system;
        }

        public IActorRef ActorOf(ActorRefBase actor, string name)
        {
            return _system.ActorOf(actor, name);
        }
    }
}
