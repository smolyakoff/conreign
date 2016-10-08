using System;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Newtonsoft.Json;
using Serilog.Core;

namespace Conreign.Experiments
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            Run().Wait();
            Console.WriteLine("Press a key to exit...");
            Console.ReadLine();
        }

        private static async Task Run()
        {
            var client = await GameClient.Initialize("OrleansClientConfiguration.xml");
            var connection = await client.Connect(Guid.NewGuid());
            Console.WriteLine($"Connection id: {connection.Id}");
            connection.Events.Subscribe(new Logger());
            var user = connection.Login();

            var player = await user.JoinRoom("conreign");
            await player.Write("blabla");
        }
    }

    public class Logger : IObserver<object>
    {
        public void OnNext(object value)
        {
            Console.WriteLine($"Next: {JsonConvert.SerializeObject(value)}");
        }

        public void OnError(Exception error)
        {
            Console.WriteLine(error.Message);
        }

        public void OnCompleted()
        {
            Console.WriteLine("Completed");
        }
    }
}
