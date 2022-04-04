using ServerAgent.Monitoring.Model;
using System;

namespace ServerAgent.Monitoring.Interactor
{
    public interface ITimeChecker
    {
        void Start();
        void Update(ProcessInfoModel model);

        uint ProcessingTime { get; }
        uint ThreadId { get; }
        DateTime LastReceiveTime { get; }
        bool IsStopped { get; }
    }

    public static class TimeCheckerFactory
    {
        public static ITimeChecker Create(DetectTimeModel detectTime)
        {
            switch (detectTime.Checker)
            {
                case "datetime":
                    return new DateTimeCheker(detectTime);
                case "timegettime":
                    return new TimeGetTimeChecker(detectTime);
                default:
                    return null;
            }
        }
    }
}
