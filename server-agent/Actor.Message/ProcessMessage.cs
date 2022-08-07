using Newtonsoft.Json;

namespace ServerAgent.Actor.Message
{
    public class ProcessKillRequest
    {
        public string Command { get; set; }
    }

    public class ProcessKillResponse
    {
        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; } = "";

        [JsonProperty("exitCode", Required = Required.Always)]
        public int ExitCode { get; set; }

        [JsonProperty("close", Required = Required.Always)]
        public bool Close { get; set; }
    }

    public class ProcessStoppedMessage
    {
        public string ServerName { get; set; }
    }
}
