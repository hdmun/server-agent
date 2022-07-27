using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace ServerAgent.Data.Entity
{
    public class MonitoringConfig
    {
        [Key]
        public string HostName { get; set; }

        [JsonProperty("checker", Required = Required.Always)]
        public string Checker { get; set; }

        [JsonProperty("deadlock", Required = Required.Always)]
        public int DeadlockMin { get; set; }

        [JsonProperty("stopped", Required = Required.Always)]
        public int StoppedMin { get; set; }
    }
}
