using Newtonsoft.Json;
using System;

namespace ServerAgent.PubSub.Model
{
    public class ServerInfoModel
    {
        [JsonProperty("hostName", Required = Required.Always)]
        public string HostName { get; set; }

        [JsonProperty("processName", Required = Required.Always)]
        public string ProcessName { get; set; }

        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; }

        [JsonProperty("processingTime", Required = Required.Always)]
        public uint ProcessingTime { get; set; }

        [JsonProperty("threadId", Required = Required.Always)]
        public uint ThreadId { get; set; }

        public DateTime LastReceiveTime { get; set; }
    }
}
