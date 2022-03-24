using Newtonsoft.Json;

namespace ServerAgent.Monitoring.Model
{
    public class JsonConfigModel
    {
        [JsonProperty("detect", Required = Required.Always)]
        public DetectTimeModel DetectTime { get; set; }

        [JsonProperty("servers", Required = Required.Always)]
        public ServerInfoModel[] ServerInfo { get; set; }
    }
}
