using System;
using System.Threading;
using System.Threading.Tasks;

namespace test_server_dot_net
{
    class Program
    {
        static bool isRunning = false;

        static void Main(string[] args)
        {
            Console.WriteLine($"start test server for dotnet: {string.Join(", ", args)}");

            var task = Task.Run(async () =>
            {
                await Update();
            });

            task.Wait();
        }

        static async Task Update()
        {
            isRunning = true;

            while (isRunning)
            {
                await Task.Delay(1000);
            }
        }
    }
}
