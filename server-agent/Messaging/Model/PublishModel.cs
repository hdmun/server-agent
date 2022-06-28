using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ServerAgent.Messaging.Model
{
    public class PublishModel
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Topic { get; set; }

        [JsonProperty("data", Required = Required.Always)]
        public JObject Data { get; set; }
    }
}
