using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace ServerAgent.ActorLite
{
    public abstract class ActorRefBase : IActorRef
    {
        private readonly ConcurrentQueue<Mailbox> _mailboxQueue;

        public ActorContext Context { private get; set; }

        private bool _start;
        private Task _taskMailbox;

        private ActorTimer _actorTimer;

        public ActorRefBase()
        {
            _mailboxQueue = new ConcurrentQueue<Mailbox>();
            _start = false;
            _taskMailbox = new Task(() => _processMailbox());

            _actorTimer = new ActorTimer(this);
        }

        public void Start()
        {
            if (_start)
                throw new Exception("Actor is Already Start");

            _start = true;
            _taskMailbox.Start();
            OnStart();
        }

        public void Stop()
        {
            if (!_start)
                throw new Exception("Actor is Already Stop");

            _start = false;

            if (!_taskMailbox.IsCompleted)
                _taskMailbox.Wait();
            _taskMailbox.Dispose();
            _taskMailbox = null;

            _actorTimer.Stop();
        }

        public void Tell(object message)
        {
            Tell(message, this);
        }

        protected void _startSingleTimer(object message, double interval)
        {
            _actorTimer.Start(message, interval);
        }

        protected abstract void OnStart();
        protected abstract void OnReceive(object message);

        private void Tell(object message, IActorRef sender)
        {
            _mailboxQueue.Enqueue(new Mailbox(message, sender));
        }

        private void _processMailbox()
        {
            try
            {
                while (_start)
                {
                    if (_mailboxQueue.IsEmpty)
                    {
                        Task.Delay(100);
                        continue;
                    }

                    if (!_mailboxQueue.TryDequeue(out Mailbox mailbox))
                        continue;

                    Context.Sender = mailbox.Sender;

                    OnReceive(mailbox.Message);
                }
            }
            catch (Exception)
            {
                _start = false;
                // OnException(ex);
            }
        }
    }
}
