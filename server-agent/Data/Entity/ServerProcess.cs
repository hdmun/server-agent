using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServerAgent.Data.Entity
{
    public class ServerProcess
    {
        [Key, Column(Order = 0)]
        public string HostName { get; set; }

        [JsonProperty("serverName", Required = Required.Always)]
        [Key, Column(Order = 1)]
        public string ServerName { get; set; }

        [JsonProperty("binaryPath", Required = Required.Always)]
        public string BinaryPath { get; set; }
    }
}
