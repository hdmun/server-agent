using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestServer
{
    class Program
    {
        static bool isRunning = false;

        static int processingTime = 600;

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
