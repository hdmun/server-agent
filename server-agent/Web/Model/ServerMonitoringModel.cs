using Newtonsoft.Json;

namespace ServerAgent.Web.Model
{
    public class ServerMonitoringModel
    {
        [JsonProperty("hostName", Required = Required.Always)]
        public string HostName { get; set; }

        [JsonProperty("on", Required = Required.Always)]
        public bool On { get; set; }
    }
}
