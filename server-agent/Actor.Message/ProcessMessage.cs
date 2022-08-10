using Newtonsoft.Json;

namespace ServerAgent.Actor.Message
{
    public class ProcessStateReqeuest
    {
        public string ServerName { get; set; }
    }

    public class ProcessStateResponse
    {
        [JsonProperty("threadId", Required = Required.Always)]
        public uint ThreadId { get; set; } = 0;

        [JsonProperty("processingTime", Required = Required.Always)]
        public uint ProcessingTime { get; set; } = 0;

        [JsonProperty("receiveTime", Required = Required.Always)]
        public string ReceiveTime { get; set; } = "";
    }

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
