﻿using server_agent.Data.Model;
using server_agent.Data.Provider;
using System;
using System.Collections.Generic;

namespace server_agent.Data
{
    public class DataConnector : IDisposable
    {
        private readonly IDataProvider provider;

        public DataConnector(IDataProvider provider)
        {
            this.provider = provider;
        }

        public bool Open()
        {
            return provider.Open();
        }

        public List<ServerInfoModel> ServerInfo() => provider.ServerInfo;

        public DetectTimeModel DetectTime() => provider.DetectTime;

        public void Dispose()
        {
            provider?.Close();
        }
    }
}
