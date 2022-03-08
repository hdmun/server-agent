using Newtonsoft.Json;

namespace server_agent.Network.Model
{
    public class MonitoringModel
    {
        [JsonProperty("on", Required = Required.Always)]
        public bool On { get; set; }
    }
}
