using Newtonsoft.Json;

namespace ServerAgent.Data.Model
{
    public class ServerMonitoringRequest
    {
        [JsonProperty("hostName", Required = Required.Always)]
        public string HostName { get; set; }

        [JsonProperty("on", Required = Required.Always)]
        public bool On { get; set; }
    }
}
