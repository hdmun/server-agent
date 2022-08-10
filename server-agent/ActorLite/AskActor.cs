using System;
using System.Threading.Tasks;

namespace ServerAgent.ActorLite
{
    public class AskActor<T> : ActorRefBase
    {
        private readonly TaskCompletionSource<T> _tcs;

        public AskActor(TaskCompletionSource<T> tcs)
        {
            _tcs = tcs;
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case T t:
                    _tcs.SetResult(t);
                    break;
                default:
                    var msg = $"Received message of type [{message.GetType()}] - Invalid message in {GetType().FullName}";
                    _tcs.SetException(new Exception(msg));
                    Logger.Error(msg);
                    break;
            }
        }
    }
}
