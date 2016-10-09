using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Contracts.Presence.Events;
using Newtonsoft.Json;

namespace Conreign.Experiments
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            var tasks = Enumerable.Range(0, 3)
                .Select(x => Run(x == 0))
                .ToArray();
            Task.WaitAll(tasks);
            Console.WriteLine("Press a key to exit...");
            Console.ReadLine();
        }

        private static async Task Run(bool isLeader)
        {
            var random = new Random();
            var client = await GameClient.Initialize("OrleansClientConfiguration.xml");
            var logger = new Logger();
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                Console.WriteLine($"Connection id: {connection.Id}");
                connection.Events.Subscribe(logger);
                var user = connection.Login();

                var player = await user.JoinRoom("conreign");
                await player.Write("Hello, world!");
                await player.UpdateOptions(new PlayerOptionsData
                {
                    Nickname = "username",
                    Color = "#020000"
                });
                Thread.Sleep(random.Next(500));
                var room = await player.GetState();
                logger.OnNext(room);
                if (isLeader)
                {
                    Thread.Sleep(500);
                    await player.StartGame();
                }
                else
                {
                    var started = await connection.WaitFor<GameStarted>();
                    Console.WriteLine("Now it's started!");
                }  
            }
        }
    }

    public class Logger : IObserver<object>
    {
        public void OnNext(object value)
        {
            Console.WriteLine();
            //Console.WriteLine(
            //    JsonConvert.SerializeObject(
            //        value,
            //        Formatting.Indented,
            //        new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.Auto}
            //        )
            //    );
            Console.WriteLine(value.GetType().Name);
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
