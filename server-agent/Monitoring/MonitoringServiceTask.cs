using log4net;
using System;
using System.Threading.Tasks;

namespace ServerAgent.Monitoring
{
    public class MonitoringServiceTask : IServiceTask
    {
        private readonly ILog logger;
        private readonly IMonitoringContext context;
        private Task taskJob;
        private bool isRunning;

        public MonitoringServiceTask(IMonitoringContext context)
        {
            logger = LogManager.GetLogger(typeof(MonitoringServiceTask));

            this.context = context;
            taskJob = null;
            isRunning = false;
        }

        public void OnStart()
        {
            logger.Info("starting monitoring service task");

            // load target process info

            isRunning = true;
            taskJob = Task.Run(async () =>
            {
                await MonitoringJob();
            });
        }

        public void OnStop()
        {
            logger.Info("stopping monitoring service task");

            isRunning = false;
            taskJob.Wait();
        }

        private async Task MonitoringJob()
        {
            while (isRunning)
            {
                await Task.Delay(1000);

                try
                {
                    context.OnMonitoring();
                }
                catch (Exception ex)
                {
                    logger.Error("Exception - IMonitoringContext.OnMonitoring", ex);
                }

                if (!context.Monitoring)
                {
                    continue;
                }

                foreach (var process in context.Processes)
                {
                    try
                    {
                        process.OnMonitoring();
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Exception - Process.OnMonitoring", ex);
                    }
                }
            }
        }
    }
}
