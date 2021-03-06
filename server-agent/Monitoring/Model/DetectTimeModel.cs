using Newtonsoft.Json;

namespace ServerAgent.Monitoring.Model
{
    public class DetectTimeModel
    {
        [JsonProperty("deadlock", Required = Required.Always)]
        public uint DeadlockMin { get; set; }

        [JsonProperty("stopped", Required = Required.Always)]
        public uint StoppedMin { get; set; }

        [JsonProperty("checker", Required = Required.Always)]
        public string Checker { get; set; }
    }
}
