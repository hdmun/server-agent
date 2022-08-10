using Newtonsoft.Json;

namespace ServerAgent.Actor.Message
{
    public class HeartbeatMessage
    {
        [JsonProperty("threadId", Required = Required.Always)]
        public uint ThreadId { get; set; } = 0;

        [JsonProperty("processingTime", Required = Required.Always)]
        public uint ProcessingTime { get; set; } = 0;
    }
}
