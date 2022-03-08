using Newtonsoft.Json;
using System;

namespace server_agent.Monitoring.Model
{
    public class ProcessInfoModel
    {
        [JsonProperty("ProcessingTime", Required = Required.Always)]
        public uint ProcessingTime { get; set; }

        [JsonProperty("ThreadId", Required = Required.Always)]
        public uint ThreadId { get; set; }

        public DateTime LastReceiveTime { get; set; }
    }
}
