using Newtonsoft.Json;

namespace server_agent.Data.Model
{
    public class DetectTimeModel
    {
        [JsonProperty("deadlock", Required = Required.Always)]
        public uint DeadlockMin { get; set; }

        [JsonProperty("stopped", Required = Required.Always)]
        public uint StoppedMin { get; set; }
    }
}
