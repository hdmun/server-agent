using Newtonsoft.Json.Linq;
using ServerAgent.Data.Entity;
using System.IO;
using System.Net;

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
            var token = jsonObj.SelectToken("servers");
            var processes = token.ToObject<ServerProcess[]>();
            foreach (var process in processes)
                process.HostName = Dns.GetHostName();
            return processes;
        }

        public MonitoringConfig FindMonitoringConfig(string _)
        {
            var jsonObj = _jsonObjectFromConfig();
            var token = jsonObj.SelectToken("monitoring");
            return token.ToObject<MonitoringConfig>();
        }
    }
}
