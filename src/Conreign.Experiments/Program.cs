using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Gameplay.AI;

namespace Conreign.Experiments
{
    public class Program
    {
        public static List<ConsoleColor> Colors = new List<ConsoleColor>
        {
            ConsoleColor.Cyan,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkMagenta,
            ConsoleColor.DarkGreen
        };

        internal static void Main(string[] args)
        {
            var tasks = Enumerable.Range(0, 7)
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
                    new LogBehaviour(Colors[n]),
                    new JoinRoomBehaviour(
                        "conreign-bots",
                        isLeader ? "Leader" : $"Bot-{n}", isLeader ? TimeSpan.Zero : TimeSpan.FromSeconds(2)),
                    new BattleBehaviour(new NaiveBotBattleStrategy(options))
                };
                if (isLeader)
                {
                    behaviours.Add(new StartGameBehaviour(7));
                }
                var login = connection.Login();
                var bot = Bot.Create(login.UserId, login.User, behaviours);
                var i = 0;
                connection.Events
                    .SelectMany(async e =>
                    {
                        await bot.Handle(e);
                        return i;
                    })
                    .Subscribe(e => {}, e => Console.WriteLine(e.ToString()), () => {Console.WriteLine("Completed");});
                await bot.Start();
                await connection.WaitFor<GameEnded>(TimeSpan.FromMinutes(30));
            }
        }
    }
}
