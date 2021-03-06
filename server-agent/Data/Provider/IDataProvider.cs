using ServerAgent.Monitoring.Model;
using System.Collections.Generic;

namespace ServerAgent.Data.Provider
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
