using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace server_agent.Monitoring
{
    public class MonitoringService : ServiceBase
    {
        private readonly IMonitoringContext context;
        private Task taskJob;
        private bool isRunning;

        public MonitoringService(IMonitoringContext context)
        {
            this.context = context;
            taskJob = null;
            isRunning = false;
        }

        protected override void OnStart(string[] args)
        {
            Debug.WriteLine("MonitoringService.OnStart");

            // load target process info

            isRunning = true;
            taskJob = Task.Run(async () =>
            {
                await MonitoringJob();
            });
        }

        protected override void OnStop()
        {
            Debug.WriteLine("MonitoringService.OnStop");

            isRunning = false;
            taskJob.Wait();
        }

        private async Task MonitoringJob()
        {
            while (isRunning)
            {
                await Task.Delay(1000);

                if (!context.Monitoring)
                {
                    continue;
                }

                foreach (var process in context.Processes)
                {
                    process.OnMonitoring();
                }

                context.OnMonitoring();
            }
        }
    }
}
