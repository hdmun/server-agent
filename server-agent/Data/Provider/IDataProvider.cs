using ServerAgent.Data.Entity;

namespace ServerAgent.Data.Provider
{
    public interface IDataProvider
    {
        ServerProcess[] FindProcesses(string hostName);
        MonitoringConfig FindMonitoringConfig(string hostName);
    }

    public static class DataProviderFactory
    {
        public static IDataProvider Create(string providerName)
        {
            switch (providerName)
            {
                case "sql":
                    return new SqlProvider();
                case "json":
                    return new JsonProvider();
                default:
                    return null;
            }
        }
    }
}
