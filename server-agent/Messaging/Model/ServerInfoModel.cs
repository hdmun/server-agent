using Newtonsoft.Json;
using System;

namespace ServerAgent.Messaging.Model
{
    public class ServerInfoModel
    {
        [JsonProperty("hostName", Required = Required.Always)]
        public string HostName { get; set; }

        [JsonProperty("processName", Required = Required.Always)]
        public string ProcessName { get; set; }

        [JsonProperty("serverName", Required = Required.Always)]
        public string ServerName { get; set; }

        [JsonProperty("alive", Required = Required.Always)]
        public bool IsAlive { get; set; }

        [JsonProperty("processingTime", Required = Required.Always)]
        public uint ProcessingTime { get; set; }

        [JsonProperty("threadId", Required = Required.Always)]
        public uint ThreadId { get; set; }

        [JsonProperty("lastReceiveTime", Required = Required.Always)]
        public DateTime LastReceiveTime { get; set; }
    }
}
