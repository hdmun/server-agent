using ServerAgent.Monitoring.Model;
using System;

namespace ServerAgent.Monitoring.Interactor
{
    public class DateTimeCheker : ITimeChecker
    {
        private readonly DetectTimeModel detectTime;
        private readonly ProcessInfoModel processInfo = new ProcessInfoModel();

        public DateTimeCheker(DetectTimeModel detectTime)
        {
            this.detectTime = detectTime;
        }

        public void Start()
        {
            processInfo.ProcessingTime = 0;
            processInfo.ThreadId = 0;
            processInfo.LastReceiveTime = DateTime.Now;
        }

        public void Update(ProcessInfoModel model)
        {
            processInfo.ProcessingTime = model.ProcessingTime / 60;  // second
            processInfo.ThreadId = model.ThreadId;
            processInfo.LastReceiveTime = DateTime.Now;
        }

        public uint ProcessingTime => processInfo.ProcessingTime;

        public uint ThreadId => processInfo.ThreadId;

        public DateTime LastReceiveTime => processInfo.LastReceiveTime;

        public bool IsStopped
        {
            get
            {
                if (processInfo.ProcessingTime > detectTime.StoppedMin)
                    return true;

                DateTime lastRecvTime = processInfo.LastReceiveTime;
                var stoppedReciveTime = lastRecvTime.AddMinutes(detectTime.StoppedMin);
                if (stoppedReciveTime <= DateTime.Now)
                    return true;

                var stoppedProcessingTime = lastRecvTime.AddMinutes(detectTime.StoppedMin - processInfo.ProcessingTime);
                if (stoppedProcessingTime <= DateTime.Now)
                    return true;

                return false;
            }
        }
    }
}
