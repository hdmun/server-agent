using ServerAgent.Monitoring.Interactor;
using System.Collections.Generic;

namespace ServerAgent.Monitoring
{
    public interface IMonitoringContext
    {
        bool Monitoring { get; set; }

        List<ServerProcess> Processes { get; }

        void OnMonitoring();
    }
}
