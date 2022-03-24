﻿using Newtonsoft.Json;

namespace ServerAgent.PubSub.Model
{
    public class HostInfoModel
    {
        [JsonProperty("hostName", Required = Required.Always)]
        public string HostName { get; set; }

        [JsonProperty("monitoring", Required = Required.Always)]
        public bool Monitoring { get; set; }
    }
}
