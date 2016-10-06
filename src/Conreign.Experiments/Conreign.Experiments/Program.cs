using System;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Experiments
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Run().Wait();
            Console.ReadLine();
        }

        private static async Task Run()
        {
            var client = await GameClient.Initialize("OrleansClientConfiguration.xml");
        }

        private static void OnError(object sender, Exception e)
        {
            Console.WriteLine($"Client stream error: {e.Message}");
        }

        private static void OnMessage(object sender, MessageEnvelope e)
        {
            foreach (var connectionId in e.ConnectionIds)
            {
                Console.WriteLine($"[{connectionId}] Message received: {e.Message.GetType().FullName}.");
            }
        }
    }
}
