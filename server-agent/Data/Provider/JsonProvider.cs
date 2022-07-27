using Newtonsoft.Json.Linq;
using ServerAgent.Data.Entity;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ServerAgent.Data.Provider
{
    public class JsonProvider : IDataProvider
    {
        private readonly string ConfigFilePath = "config.json";

        public JsonProvider()
        {
        }

        private JObject _jsonObjectFromConfig()
        {
            if (!File.Exists(ConfigFilePath))
                throw new FileNotFoundException($"`{ConfigFilePath}` not exists");

            var jsonText = File.ReadAllText(ConfigFilePath);
            return JObject.Parse(jsonText);
        }

        public ServerProcess[] FindProcesses(string _)
        {
            var jsonObj = _jsonObjectFromConfig();
            return jsonObj["servers"].Values<ServerProcess>().ToArray();
        }

        public MonitoringConfig FindMonitoringConfig(string _)
        {
            var jsonObj = _jsonObjectFromConfig();
            return jsonObj["monitoring"].Value<MonitoringConfig>();
        }
    }
}
