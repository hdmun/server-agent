using Newtonsoft.Json;

namespace server_agent.Network.Model
{
    public class ResponseModel
    {
        [JsonProperty("success", Required = Required.Always)]
        public bool Success { get; set; }

        [JsonProperty("message", Required = Required.Always)]
        public string Message { get; set; }

        [JsonProperty("request", Required = Required.Always)]
        public string Request { get; set; }
    }
}
