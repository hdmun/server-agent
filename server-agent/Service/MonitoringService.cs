using System.Diagnostics;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace server_agent
{
    public class MonitoringService : ServiceBase
    {
        private readonly IContext context;
        private Task taskJob;
        private bool isRunning;

        public MonitoringService(IContext context)
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
            }
        }
    }
}
