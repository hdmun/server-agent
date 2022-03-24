using log4net;
using System.ServiceProcess;
using System.Threading.Tasks;

namespace ServerAgent.Monitoring
{
    public class MonitoringService : ServiceBase
    {
        private readonly ILog logger;
        private readonly IMonitoringContext context;
        private Task taskJob;
        private bool isRunning;

        public MonitoringService(IMonitoringContext context)
        {
            logger = LogManager.GetLogger(typeof(MonitoringService));

            this.context = context;
            taskJob = null;
            isRunning = false;
        }

        protected override void OnStart(string[] args)
        {
            logger.Info("starting service");

            // load target process info

            isRunning = true;
            taskJob = Task.Run(async () =>
            {
                await MonitoringJob();
            });
        }

        protected override void OnStop()
        {
            logger.Info("stopping service");

            isRunning = false;
            taskJob.Wait();
        }

        private async Task MonitoringJob()
        {
            while (isRunning)
            {
                await Task.Delay(1000);

                context.OnMonitoring();

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
