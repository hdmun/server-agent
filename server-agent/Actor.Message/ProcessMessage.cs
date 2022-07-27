namespace ServerAgent.Actor.Message
{
    public class ProcessKillMessage
    {
        public string Command { get; set; }
    }

    public class ProcessKillResponseMessage
    {
        public string ServerName { get; set; }
        public int ExitCode { get; set; }
        public bool Close { get; set; }
    }

    public class ProcessStoppedMessage
    {
        public string ServerName { get; set; }
    }
}
