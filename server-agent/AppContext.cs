using server_agent.Data;
using server_agent.Data.Provider;

namespace server_agent
{
    public class AppContext : IContext
    {
        private bool monitoring;
        private DataConnector dataConnector;

        public AppContext()
        {
            monitoring = true;
            dataConnector = new DataConnector(DataProviderFactory.Create("json"));
        }

        public bool Monitoring
        {
            get
            {
                lock (this)
                    return monitoring;
            }
            set
            {
                lock (this)
                    monitoring = value;
            }
        }

        public void OnStart()
        {
            if (!dataConnector.Open())
            {
                return;  // throw exception
            }
        }
    }
}
