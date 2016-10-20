using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Gameplay.AI;
using Conreign.Core.Gameplay.AI.Behaviours;
using Serilog;

namespace Conreign.Experiments
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            const int rooms = 3;
            const int players = 3;
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .MinimumLevel.Debug()
                .CreateLogger();
            var tasks = Enumerable.Range(0, rooms)
                .Select(i => $"conreign-bots-{i}")
                .SelectMany((roomId, i) => Enumerable.Range(0, players).Select(k => Run(roomId, k, players)))
                .ToArray();
            Task.WaitAll(tasks);
            Console.WriteLine("Press a key to exit...");
            Console.ReadLine();
        }

        private static async Task Run(string roomId, int n, int total)
        {
            var isLeader = n == 0;
            var client = await GameClient.Initialize("OrleansClientConfiguration.xml");
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                var name = isLeader ? "Leader" : $"Bot-{n}";
                Console.WriteLine($"Connection id: {connection.Id}");
                var options = new NaiveBotBattleStrategyOptions(0.8, 0.2, 1);
                var behaviours = new List<IBotBehaviour>
                {
                    new LogBehaviour(),
                    new JoinRoomBehaviour(roomId, name, isLeader ? TimeSpan.Zero : TimeSpan.FromSeconds(0.5)),
                    new BattleBehaviour(new NaiveBotBattleStrategy(options)),
                    new StopOnGameEndBehaviour()
                };
                if (isLeader)
                {
                    behaviours.Add(new StartGameBehaviour(total));
                }
                var login = connection.Login();
                var bot = Bot.Create(name, login.UserId, login.User, behaviours);
                connection.Events
                    .SelectMany(async e =>
                    {
                        await bot.Handle(e);
                        return Unit.Default;
                    })
                    .Subscribe();
                await bot.Start();
                await bot.Completion;
                //await connection.WaitFor<GameEnded>(TimeSpan.FromMinutes(30));
            }
        }
    }
}
