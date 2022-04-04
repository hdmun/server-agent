using Newtonsoft.Json;

namespace ServerAgent.Web.Model
{
    public class ServerKillResponseModel
    {
        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; } = "";

        [JsonProperty("exitCode", Required = Required.Always)]
        public int ExitCode { get; set; }

        [JsonProperty("close", Required = Required.Always)]
        public bool Close { get; set; }
    }
}
