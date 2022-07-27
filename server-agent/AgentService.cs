using log4net;
using ServerAgent.Actor;
using ServerAgent.ActorLite;
using ServerAgent.Data.Provider;
using System.Configuration;
using Topshelf;

namespace ServerAgent
{
    public class AgentService : ServiceControl
    {
        private readonly ILog logger;
        private readonly IDataProvider _dataProvider;

        private readonly ActorSystem _actorSystem;
        private readonly IActorRef _messagePublishActor;
        private readonly IActorRef _monitoringActor;

        public AgentService()
        {
            logger = LogManager.GetLogger(typeof(AgentService));

            var providerName = ConfigurationManager.AppSettings["DataProvider"];
            _dataProvider = DataProviderFactory.Create(providerName);

            _actorSystem = ActorSystem.Create("AgentService");

            var publisherAddr = ConfigurationManager.AppSettings["PublisherAddr"];
            _messagePublishActor = _actorSystem.ActorOf(new MessagePublishActor(publisherAddr), "MessagePublishActor");

            _monitoringActor = _actorSystem.ActorOf(new MonitoringActor(_dataProvider), "MonitoringActor");

            var bindUrl = ConfigurationManager.AppSettings["HttpUrl"];
            _actorSystem.ActorOf(new HttpServerActor(bindUrl, _monitoringActor), "HttpServerActor");
        }

        bool ServiceControl.Start(HostControl hostControl)
        {
            logger.Info("start agent service");
            return true;
        }

        bool ServiceControl.Stop(HostControl hostControl)
        {
            _actorSystem.Dispose();
            logger.Info("stop agent service");
            return true;
        }
    }
}
