using ServerAgent.Data.Entity;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;

namespace ServerAgent.Data.Provider
{
    public class SqlProvider : DbContext, IDataProvider
    {
        public SqlProvider()
            : base("Initial Catalog=ServerAgent;Server=localhost;Integrated Security=SSPI")
        {
        }

        public DbSet<ServerProcess> ServerProcesses { get; set; }
        public DbSet<MonitoringConfig> MonitoringConfigs { get; set; }

        public ServerProcess[] FindProcesses(string hostName)
        {
            return (from process in ServerProcesses
                    where process.HostName == hostName
                    select process).ToArray();
        }

        public MonitoringConfig FindMonitoringConfig(string hostName)
        {
            return MonitoringConfigs.Find(hostName);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
