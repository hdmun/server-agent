using Newtonsoft.Json;

namespace ServerAgent.Web.Model
{
    public class ServerKillRequestModel
    {
        [JsonProperty("killCommand", Required = Required.Always)]
        public string KillCommand { get; set; } = "";

        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; } = "";
    }
}
