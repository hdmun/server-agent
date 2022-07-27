using Newtonsoft.Json;

namespace ServerAgent.Actor.Message
{
    public class PublishMessage
    {
        [JsonProperty("type", Required = Required.Always)]
        public string Topic { get; set; }

        [JsonProperty("data", Required = Required.Always)]
        public object Data { get; set; }
    }
}
