using server_agent.Interactor;
using System.Collections.Generic;

namespace server_agent
{
    public interface IContext
    {
        bool Monitoring { get; set; }

        List<ServerProcess> Processes { get; }

        void OnStart();
    }
}
