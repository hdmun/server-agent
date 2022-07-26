using System;
using System.Collections.Generic;

namespace ServerAgent.ActorLite
{
    public class ActorSystem : IDisposable
    {
        public static ActorSystem Create(string name)
        {
            var sys = new ActorSystem(name);
            // sys.Start();
            return sys;
        }

        private readonly string _name;
        private bool _disposed;

        private Dictionary<string, IActorRef> _actors;

        private ActorSystem(string name)
        {
            _name = name;
            _disposed = false;

            _actors = new Dictionary<string, IActorRef>();
        }

        public IActorRef ActorOf(ActorRefBase actor, string name, IActorRef parent = null)
        {
            if (_actors.ContainsKey(name))
            {
                throw new Exception($"duplicate actor name: {name}");
            }

            _actors.Add(name, actor);

            actor.Context = new ActorContext(this, parent);
            actor.Start();
            return actor;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var actor in _actors.Values)
                {
                    actor.Stop();
                }
            }

            _disposed = true;
        }
    }
}
