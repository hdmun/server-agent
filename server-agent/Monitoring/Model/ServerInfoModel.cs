using Newtonsoft.Json;

namespace ServerAgent.Monitoring.Model
{
    public class ServerInfoModel
    {
        [JsonProperty("binaryPath", Required = Required.Always)]
        public string BinaryPath { get; set; }

        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; }
    }
}
