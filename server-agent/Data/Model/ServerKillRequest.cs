using Newtonsoft.Json;

namespace ServerAgent.Data.Model
{
    public class ServerKillRequest
    {
        [JsonProperty("killCommand", Required = Required.Always)]
        public string KillCommand { get; set; } = "";

        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; } = "";
    }
}
