using Newtonsoft.Json;
using ServerAgent.Monitoring.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServerAgent.Data.Provider
{
    public class JsonProvider : IDataProvider
    {
        private readonly string configFilePath = "config.json";

        public JsonProvider()
        {
        }

        public bool Open()
        {
            try
            {
                if (!File.Exists(configFilePath))
                {
                    // error message
                    return false;
                }

                string jsonServerInfo = File.ReadAllText(configFilePath);
                JsonConfigModel jsonConfig = JsonConvert.DeserializeObject<JsonConfigModel>(jsonServerInfo);

                DetectTime = jsonConfig.DetectTime;
                ServerInfo = jsonConfig.ServerInfo.ToList();
            }
            catch (Exception)
            {
                // error message
                return false;
            }

            return true;
        }

        public void Close()
        {
            ServerInfo = null;
            DetectTime = null;
        }

        public List<ServerInfoModel> ServerInfo { get; private set; } = null;

        public DetectTimeModel DetectTime { get; private set; } = null;
    }
}
