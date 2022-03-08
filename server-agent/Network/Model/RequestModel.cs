using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace server_agent.Network.Model
{
    public class RequestModel
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; set; }

        [JsonProperty("data", Required = Required.Always)]
        public JObject Data { get; set; }
    }
}
