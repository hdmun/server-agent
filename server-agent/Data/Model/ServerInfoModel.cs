using Newtonsoft.Json;

namespace server_agent.Data.Model
{
    public class ServerInfoModel
    {
        [JsonProperty("binaryPath", Required = Required.Always)]
        public string BinaryPath { get; set; }

        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; }
    }
}
