﻿namespace server_agent.Web
{
    public interface IWebServiceContext
    {
        bool Monitoring { get; set; }

        void OnServerKill();
    }
}
