using System;
using System.Reflection;
using System.ServiceProcess;

namespace server_agent
{
    static class Program
    {
        static IContext context = new AppContext();

        /// <summary>
        /// 해당 애플리케이션의 주 진입점입니다.
        /// </summary>
        static void Main(string[] args)
        {
            context.OnStart();

            ServiceBase[] ServicesToRun = new ServiceBase[]
            {
                new MonitoringService(context)
            };

            if (Environment.UserInteractive)
            {
                RunInteractive(ServicesToRun, args);
            }
            else
            {
                ServiceBase.Run(ServicesToRun);
            }
        }

        static void RunInteractive(ServiceBase[] ServicesToRun, string[] args)
        {
            MethodInfo onStartMethod = typeof(ServiceBase)
                .GetMethod("OnStart", BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var service in ServicesToRun)
            {
                onStartMethod.Invoke(service, new object[] { args });
            }

            Console.ReadKey();

            foreach (var service in ServicesToRun)
            {
                service.Stop();
            }
        }
    }
}
