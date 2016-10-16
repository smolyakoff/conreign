using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Contracts.Presence.Events;
using Conreign.Core.Gameplay.AI;
using Newtonsoft.Json;
using Orleans.Streams;

namespace Conreign.Experiments
{
    public class Program
    {
        public static List<ConsoleColor> _colors = new List<ConsoleColor>
        {
            ConsoleColor.Cyan,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkMagenta,
            ConsoleColor.DarkGreen,
        };

        internal static void Main(string[] args)
        {
            var tasks = Enumerable.Range(0, 5)
                .Select(x => Run(x == 0, x))
                .ToArray();
            Task.WaitAll(tasks);
            Console.WriteLine("Press a key to exit...");
            Console.ReadLine();
        }

        private static async Task Run(bool isLeader, int n)
        {
            var client = await GameClient.Initialize("OrleansClientConfiguration.xml");
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                Console.WriteLine($"Connection id: {connection.Id}");
                var options = new NaiveBotBattleStrategyOptions(0.8, 0.2, 1);
                var behaviours = new List<IBotBehaviour>
                {
                    new LogBehaviour(_colors[n]),
                    new JoinRoomBehaviour(
                        "conreign-bots", 
                        isLeader ? "Leader" : $"Bot-{n}", isLeader ? TimeSpan.Zero : TimeSpan.FromSeconds(0.5)),
                    new BattleBehaviour(new NaiveBotBattleStrategy(options))
                };
                if (isLeader)
                {
                    behaviours.Add(new StartGameBehaviour(5));
                }
                var login = connection.Login();
                var bot = Bot.Create(login.UserId, login.User, behaviours);
                connection.Events
                    .SelectMany(e => Observable.FromAsync(() => bot.Handle(e)))
                    .Subscribe();
                await bot.Start();
                await connection.WaitFor<GameEnded>(TimeSpan.FromMinutes(30));
            }
        }
    }
}
