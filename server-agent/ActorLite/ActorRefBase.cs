﻿using log4net;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ServerAgent.ActorLite
{
    public abstract class ActorRefBase : IActorRef
    {
        private readonly ConcurrentQueue<Mailbox> _mailboxQueue;
        private readonly Dictionary<string, ActorTimer> _actorTimers;

        private bool _start;
        private SemaphoreSlim _semaphore;
        private Task _taskMailbox;

        public ActorContext Context { protected get; set; }

        protected ILog Logger { get; set; }
        protected IActorRef Sender { get => Context.Sender; }
        protected IActorRef Self { get => this; }

        public ActorRefBase()
        {
            _mailboxQueue = new ConcurrentQueue<Mailbox>();
            _actorTimers = new Dictionary<string, ActorTimer>();

            _start = false;
            _semaphore = new SemaphoreSlim(0);
            _taskMailbox = new Task(() => _processMailbox());

            Logger = LogManager.GetLogger(GetType());
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

            OnStop();

            _start = false;
            _semaphore.Release(1);

            if (!_taskMailbox.IsCompleted)
                _taskMailbox.Wait();
            _taskMailbox.Dispose();
            _taskMailbox = null;

            foreach (var timer in _actorTimers.Values)
                timer.Stop();
        }

        public void Tell(object message, IActorRef sender)
        {
            if (sender == null)
                sender = Self;

            _mailboxQueue.Enqueue(new Mailbox(message, sender));
            _semaphore.Release(1);
        }

        public Task<T> Ask<T>(object message)
        {
            var tcs = new TaskCompletionSource<T>();

            var askActor = new AskActor<T>(tcs);
            askActor.Context = new ActorContext(null, null);
            askActor.Start();

            Self.Tell(message, askActor);

            return tcs.Task;
        }

        protected void StartSingleTimer(string name, object message, double interval)
        {
            if (_actorTimers.ContainsKey(name))
                throw new Exception($"duplicate timer name: {name}");

            var actorTimer = new ActorTimer(this);
            actorTimer.Start(message, interval);
            _actorTimers.Add(name, actorTimer);
        }

        protected void StopSingleTimer(string name)
        {
            if (!_actorTimers.ContainsKey(name))
                throw new Exception($"duplicate timer name: {name}");

            _actorTimers[name].Stop();
            _actorTimers.Remove(name);
        }

        protected virtual void OnStart() { }
        protected virtual void OnStop() { }
        protected abstract void OnReceive(object message);

        private void _processMailbox()
        {
            try
            {
                while (_start)
                {
                    _semaphore.Wait();

                    if (_mailboxQueue.IsEmpty)
                        continue;

                    if (!_mailboxQueue.TryDequeue(out Mailbox mailbox))
                        continue;

                    Context.Sender = mailbox.Sender;

                    OnReceive(mailbox.Message);
                }
            }
            catch (Exception ex)
            {
                _start = false;
                Logger?.Error($"exception `{GetType().FullName}._processMailbox`", ex);
            }
        }
    }
}
