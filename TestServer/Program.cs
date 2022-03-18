using System;
using System.Threading;
using System.Threading.Tasks;

namespace test_server_dot_net
{
    class Program
    {
        static bool isRunning = false;

        static int processingTime = 3600;

        static void Main(string[] args)
        {
            Console.WriteLine($"start test server for dotnet: {string.Join(", ", args)}");

            Task.Run(async () => await Woker()).Wait();
        }

        static async Task Woker()
        {
            isRunning = true;

            while (isRunning)
            {
                await Task.Delay(5000);

                Console.WriteLine($"{"{"}\"ProcessingTime\": {processingTime}, \"ThreadId\": {Thread.CurrentThread.ManagedThreadId} {"}"}");
            }
        }
    }
}
