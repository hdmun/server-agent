using Newtonsoft.Json;
using System;

namespace ServerAgent.Monitoring.Model
{
    public class ProcessInfoModel
    {
        [JsonProperty("ProcessingTime", Required = Required.Always)]
        public uint ProcessingTime { get; set; } = 0;

        [JsonProperty("ThreadId", Required = Required.Always)]
        public uint ThreadId { get; set; } = 0;

        public DateTime LastReceiveTime { get; set; } = DateTime.Now;
    }
}
