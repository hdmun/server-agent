using Newtonsoft.Json;

namespace ServerAgent.Actor.Message
{
    public class MonitoringMessage
    {
        [JsonProperty("on", Required = Required.Always)]
        public bool On { get; set; } = false;
    }

    public class ServerKillRequestMessage
    {
        public string KillCommand { get; set; }
        public string ServerName { get; set; } = "";
    }

    public class ServerKillResponseMessage
    {
        public ProcessKillResponseMessage[] Servers { get; set; }
    }
}
