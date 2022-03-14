using server_agent.Monitoring.Interactor;
using System.Collections.Generic;

namespace server_agent.Monitoring
{
    public interface IMonitoringContext
    {
        bool Monitoring { get; set; }

        List<ServerProcess> Processes { get; }

        void OnMonitoring();
    }
}
