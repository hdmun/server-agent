using ServerAgent.Monitoring.Model;
using System;
using System.Runtime.InteropServices;

namespace ServerAgent.Monitoring.Interactor
{
    public class TimeGetTimeChecker : ITimeChecker
    {
        [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
        private static extern uint timeGetTime();

        private readonly DetectTimeModel detectTime;
        private readonly ProcessInfoModel processInfo = new ProcessInfoModel();

        private uint lastReceiveTime = timeGetTime();

        public TimeGetTimeChecker(DetectTimeModel detectTime)
        {
            this.detectTime = detectTime;
        }

        public void Start()
        {
            processInfo.ProcessingTime = 0;
            processInfo.ThreadId = 0;
            processInfo.LastReceiveTime = DateTime.Now;
            lastReceiveTime = timeGetTime();
        }

        public void Update(ProcessInfoModel model)
        {
            processInfo.ProcessingTime = model.ProcessingTime / 60;  // second to min
            processInfo.ThreadId = model.ThreadId;
            processInfo.LastReceiveTime = DateTime.Now;
            lastReceiveTime = timeGetTime();
        }

        public uint ProcessingTime => processInfo.ProcessingTime;

        public uint ThreadId => processInfo.ThreadId;

        public DateTime LastReceiveTime => processInfo.LastReceiveTime;

        private bool IsOverTimeTick(uint overMs)
        {
            return timeGetTime() - lastReceiveTime > overMs;
        }

        public bool IsStopped
        {
            get
            {
                if (processInfo.ProcessingTime > detectTime.StoppedMin)
                    return true;

                if (IsOverTimeTick(detectTime.StoppedMin * 60 * 1000))
                {
                    return true;
                }

                var decProcessingTime = detectTime.StoppedMin - processInfo.ProcessingTime;
                if (IsOverTimeTick(decProcessingTime * 60 * 1000))
                {
                    return true;
                }

                return false;
            }
        }
    }
}
