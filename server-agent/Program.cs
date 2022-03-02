using System;
using System.ServiceProcess;

namespace server_agent
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                AgentService agentService = new AgentService();
                agentService.TestStartupAndStop(args);
            }
            else
            {
                ServiceBase[] ServicesToRun = new ServiceBase[]
                {
                    new AgentService()
                };
                ServiceBase.Run(ServicesToRun);
            }
        }
    }
}
