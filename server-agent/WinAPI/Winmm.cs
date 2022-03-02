using System.Runtime.InteropServices;

namespace server_agent.WinAPI
{
    public class Winmm
    {
        [DllImport("winmm.dll")]
        public static extern uint timeGetTime();
    }
}
