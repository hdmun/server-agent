using server_agent.Monitoring.Interactor;
using System.Collections.Generic;

namespace server_agent
{
    public interface IContext
    {
        bool Monitoring { get; set; }

        List<ServerProcess> Processes { get; }

        void OnStart();

        void OnMonitoring();
    }
}
