using log4net;
using ServerAgent.Actor;
using ServerAgent.ActorLite;
using ServerAgent.Data.Provider;
using Topshelf;

namespace ServerAgent
{
    public class AgentService : ServiceControl
    {
        private readonly ILog _logger;
        private readonly ConfigLoader _configLoader;
        private IDataProvider _dataProvider;

        private ActorSystem _actorSystem;

        public AgentService()
        {
            _logger = LogManager.GetLogger(typeof(AgentService));
            _configLoader = new ConfigLoader();
        }

        bool ServiceControl.Start(HostControl hostControl)
        {
            _logger.Info("agent service starting...");

            if (!_configLoader.Load())
            {
                _logger.Error("failed to load config");
                return false;
            }

            _logger.Info("success to config load");

            _dataProvider = DataProviderFactory.Create(_configLoader.DataProvider);

            _actorSystem = ActorSystem.Create("AgentService");

            IActorRef monitoringActor = _actorSystem.ActorOf(
                new MonitoringActor(_dataProvider), "MonitoringActor");
            _actorSystem.ActorOf(new HttpServerActor(_configLoader.HttpBindUrl, monitoringActor), "HttpServerActor");

            _actorSystem.ActorOf(new MessagePublishActor(_configLoader.PublisherAddr), "MessagePublishActor");
            _logger.Info("success to start agent service");
            return true;
        }

        bool ServiceControl.Stop(HostControl hostControl)
        {
            _actorSystem.Dispose();
            _logger.Info("stop agent service");
            return true;
        }
    }
}
