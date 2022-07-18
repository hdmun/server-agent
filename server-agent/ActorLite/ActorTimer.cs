using System;
using System.Timers;

namespace ServerAgent.ActorLite
{
    public class ActorTimer
    {
        private readonly Timer _timer;
        private readonly IActorRef _actor;
        private object _message;

        public ActorTimer(IActorRef actor)
        {
            _timer = new Timer();
            _timer.Elapsed += _timer_Elapsed;
            _timer.AutoReset = true;

            _actor = actor;
        }

        public void Start(object message, double internval)
        {
            if (_timer.Enabled)
                throw new Exception("ActorTimer is already start ");

            if (_actor == null)
                throw new NullReferenceException("Receive Actor is null");

            _message = message;
            _timer.Interval = internval;
            _timer.Enabled = true;
        }

        public void Stop()
        {
            if (_timer.Enabled)
                _timer.Enabled = false;
        }

        private void _timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            _actor.Tell(_message);
        }
    }
}
