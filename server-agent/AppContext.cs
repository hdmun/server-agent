namespace server_agent
{
    public class AppContext : IContext
    {
        private bool monitoring;

        public AppContext()
        {
            monitoring = true;
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
    }
}
