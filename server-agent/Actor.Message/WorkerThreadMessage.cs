using Newtonsoft.Json;

namespace ServerAgent.Actor.Message
{
    public class WorkerThreadMessage
    {
        [JsonProperty("ThreadId", Required = Required.Always)]
        public uint ThreadId { get; set; } = 0;

        [JsonProperty("ProcessingTime", Required = Required.Always)]
        public uint ProcessingTime { get; set; } = 0;
    }
}
