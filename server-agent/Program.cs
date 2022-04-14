using Topshelf;

namespace ServerAgent
{
    static class Program
    {
        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        static int Main(string[] args)
        {
            return (int)HostFactory.Run(x =>
            {
                x.SetServiceName("Server Agent");
                x.SetDisplayName("Server Agent Service");
                x.SetDescription("Server Agent Service for Monitoring, Request");

                x.UseAssemblyInfoForServiceInfo();
                x.RunAsLocalSystem();
                x.StartAutomatically();
                x.Service(() => new AgentService());
                x.EnableServiceRecovery(r => r.RestartService(1));
            });
        }
    }
}
