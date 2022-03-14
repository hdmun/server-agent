using server_agent.Monitoring.Model;
using System.Collections.Generic;

namespace server_agent.Data.Provider
{
    public interface IDataProvider
    {
        bool Open();

        void Close();

        List<ServerInfoModel> ServerInfo { get; }

        DetectTimeModel DetectTime { get; }
    }

    public static class DataProviderFactory
    {
        public static IDataProvider Create(string providerName)
        {
            switch (providerName)
            {
                case "json":
                    return new JsonProvider();
                default:
                    return null;
            }
        }
    }
}
