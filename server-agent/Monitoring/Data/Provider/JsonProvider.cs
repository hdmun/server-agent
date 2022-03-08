using Newtonsoft.Json;
using server_agent.Monitoring.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace server_agent.Monitoring.Data.Provider
{
    public class JsonProvider : IDataProvider
    {
        private readonly string serverInfoFilePath = "server-info.json";
        private readonly string detectDataFilePath = "detect-data.json";

        public JsonProvider()
        {
        }

        public bool Open()
        {
            try
            {
                if (!File.Exists(serverInfoFilePath))
                {
                    // error message
                    return false;
                }

                if (!File.Exists(detectDataFilePath))
                {
                    // error message
                    return false;
                }

                string jsonServerInfo = File.ReadAllText(serverInfoFilePath);
                ServerInfo = JsonConvert.DeserializeObject<ServerInfoModel[]>(jsonServerInfo).ToList();

                string jsonDetectData = File.ReadAllText(detectDataFilePath);
                DetectTime = JsonConvert.DeserializeObject<DetectTimeModel>(jsonDetectData);
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
