using log4net;
using ServerAgent.Actor.Message;
using ServerAgent.ActorLite;
using ServerAgent.Data.Entity;
using System;
using System.Runtime.InteropServices;

namespace ServerAgent.Actor
{
    public class TimeCheckActorFactory
    {
        public static IActorRef Create(MonitoringConfig monitoringConfig)
        {
            switch (monitoringConfig.Checker)
            {
                case "datetime":
                    return new DateTimeCheckActor(monitoringConfig);
                case "timegettime":
                    return new TickTimeCheckActor(monitoringConfig);
                default:
                    throw new ArgumentException($"Invalid Argument Checker: {monitoringConfig.Checker}");
            }
        }
    }

    public class DateTimeCheckActor : ActorRefBase
    {
        class CheckModel
        {
            public uint ThreadId { get; set; } = 0;
            public uint ProcessingTime { get; set; } = 0;
            public DateTime LastReceiveTime { get; set; } = DateTime.Now;
        }

        private readonly MonitoringConfig _config;
        private readonly CheckModel _checkModel;

        public DateTimeCheckActor(MonitoringConfig config)
        {
            _config = config;
            _checkModel = new CheckModel();
        }

        protected override void OnStart()
        {
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AliveCheckMessage _:
                    OnAliveCheckMessage();
                    break;
                case WorkerThreadMessage _message:
                    OnWorkerThreadMessage(_message);
                    break;
                default:
                    Logger.Error($"Received message of type [{message.GetType()}] - Invalid message in DateTimeCheckActor");
                    break;
            }
        }

        private void OnAliveCheckMessage()
        {
            if (IsStopped())
            {
                Context.Parent?.Tell(new ProcessStoppedMessage(), Self);
            }
        }

        private void OnWorkerThreadMessage(WorkerThreadMessage message)
        {
            _checkModel.ThreadId = message.ThreadId;
            _checkModel.ProcessingTime = message.ProcessingTime;
            _checkModel.LastReceiveTime = DateTime.Now;
        }

        private bool IsStopped()
        {
            if (_checkModel.ProcessingTime > _config.StoppedMin)
                return true;

            DateTime lastRecvTime = _checkModel.LastReceiveTime;
            var stoppedReciveTime = lastRecvTime.AddMinutes(_config.StoppedMin);
            if (stoppedReciveTime <= DateTime.Now)
                return true;

            var stoppedProcessingTime = lastRecvTime.AddMinutes(_config.StoppedMin - _checkModel.ProcessingTime);
            if (stoppedProcessingTime <= DateTime.Now)
                return true;

            return false;
        }
    }

    public class TickTimeCheckActor : ActorRefBase
    {
        class CheckModel
        {
            public uint ThreadId { get; set; } = 0;
            public uint ProcessingTime { get; set; } = 0;
            public uint LastReceive { get; set; } = timeGetTime();
        }

        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        private static extern uint timeGetTime();

        private readonly MonitoringConfig _config;
        private readonly CheckModel _checkModel;

        public TickTimeCheckActor(MonitoringConfig config)
        {
            _config = config;
            _checkModel = new CheckModel();
        }

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case AliveCheckMessage _:
                    OnAliveCheckMessage();
                    break;
                case WorkerThreadMessage _message:
                    OnWorkerThreadMessage(_message);
                    break;
                default:
                    Logger.Error($"Received message of type [{message.GetType()}] - Invalid message in DateTimeCheckActor");
                    break;
            }
        }

        private void OnAliveCheckMessage()
        {
            if (IsStopped())
            {
                Context.Parent?.Tell(new ProcessStoppedMessage(), Self);
            }
        }

        private void OnWorkerThreadMessage(WorkerThreadMessage message)
        {
            _checkModel.ThreadId = message.ThreadId;
            _checkModel.ProcessingTime = message.ProcessingTime;
            _checkModel.LastReceive = timeGetTime();
        }

        private bool IsOverTimeTick(uint milliseconds)
        {
            return timeGetTime() - _checkModel.LastReceive > milliseconds;
        }

        private bool IsStopped()
        {
            if (_checkModel.ProcessingTime > _config.StoppedMin)
                return true;

            if (IsOverTimeTick((uint)_config.StoppedMin * 60 * 1000))
            {
                return true;
            }

            var decProcessingTime = _config.StoppedMin - _checkModel.ProcessingTime;
            if (IsOverTimeTick((uint)decProcessingTime * 60 * 1000))
            {
                return true;
            }

            return false;
        }
    }
}
