namespace ServerAgent.Actor.Message
{
    public class MonitoringMessage
    {
        public bool On { get; set; } = false;
    }

    public class ServerKillRequestMessage
    {
        public string KillCommand { get; set; }
        public string ServerName { get; set; } = "";
    }

    public class ServerKillResponseMessage
    {
        public ProcessKillResponseMessage[] Servers { get; set; }
    }
}
