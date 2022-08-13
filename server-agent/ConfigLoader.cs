using log4net;
using System.Configuration;

namespace ServerAgent
{
    public class ConfigLoader
    {
        private readonly ILog _logger;

        public string DataProvider { get; private set; } = null;
        public string PublisherAddr { get; private set; } = null;
        public string HttpBindUrl { get; private set; } = null;

        public ConfigLoader()
        {
            _logger = LogManager.GetLogger(typeof(ConfigLoader));
        }

        public bool Load()
        {
            bool success = true;
            do
            {
                DataProvider = ConfigurationManager.AppSettings.Get("DataProvider");
                if (DataProvider == null)
                {
                    _logger.Error("not found `DataProvider`");
                    success = false;
                }

                PublisherAddr = ConfigurationManager.AppSettings.Get("PublisherAddr");
                if (PublisherAddr == null)
                {
                    _logger.Error("not found `PublisherAddr`");
                    success = false;
                }

                HttpBindUrl = ConfigurationManager.AppSettings.Get("HttpUrl");
                if (HttpBindUrl == null)
                {
                    _logger.Error("not found `HttpUrl`");
                    success = false;
                }
            } while (false);

            return success;
        }
    }
}
