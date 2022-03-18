using Newtonsoft.Json;

namespace server_agent.Web.Model
{
    public class ServerMonitoringModel
    {
        [JsonProperty("on", Required = Required.Always)]
        public bool On { get; set; }
    }
}
