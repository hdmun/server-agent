using Newtonsoft.Json;

namespace ServerAgent.Actor.Message
{
    public class MonitoringMessage
    {
        [JsonProperty("hostName", Required = Required.Always)]
        public string HostName { get; set; }

        [JsonProperty("on", Required = Required.Always)]
        public bool On { get; set; }
    }

    public class ServerKillRequest
    {
        [JsonProperty("killCommand", Required = Required.Always)]
        public string KillCommand { get; set; } = "";

        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; } = "";
    }

    public class ServerKillResponse
    {
        public ProcessKillResponse[] Servers { get; set; }
    }
}
