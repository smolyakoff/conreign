using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Client.Messages;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Gameplay.AI;
using Conreign.Core.Gameplay.AI.Behaviours;
using Serilog;
using Serilog.Events;

namespace Conreign.Experiments
{
    public class Program
    {
        internal static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .MinimumLevel.Debug()
                .CreateLogger();
            //Simulate();
            TestHandler();
            Console.WriteLine("Press a key to exit...");
            Console.ReadLine();
        }

        private static void TestHandler()
        {
            RunHandler().Wait();
        }

        private static void Simulate()
        {
            const int rooms = 3;
            const int players = 3;
            var tasks = Enumerable.Range(0, rooms)
                .Select(i => $"conreign-bots-{i}")
                .SelectMany((roomId, i) => Enumerable.Range(0, players).Select(k => RunBot(roomId, k, players)))
                .ToArray();
            Task.WaitAll(tasks);
        }

        private static async Task RunHandler()
        {
            var client = await GameClient.Initialize("OrleansClientConfiguration.xml");
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                var handler = new GameHandler(connection);
                handler.Events.Subscribe(WriteEvent);
                var meta = new Metadata();
                var loginResponse = await handler.Handle(new LoginCommand(), meta);
                Log.Debug("Received login response: {@LoginResponse}", loginResponse);
                meta = new Metadata {AccessToken = loginResponse.AccessToken};
                var joinRoom = new JoinRoomCommand {RoomId = "conreign"};
                await handler.Handle(joinRoom, meta);
                var updatePlayer = new UpdatePlayerOptionsCommand
                {
                    RoomId = "conreign",
                    Nickname = "smolyakoff",
                    Color = "#660101"
                };
                await handler.Handle(updatePlayer, meta);
            }
        }

        private static async Task RunBot(string roomId, int n, int total)
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
                var loginResult = await connection.Login();
                var bot = Bot.Create(connection.Id, name, loginResult.UserId, loginResult.User, behaviours);
                connection.Events
                    .SelectMany(async e =>
                    {
                        await bot.Handle(e);
                        return Unit.Default;
                    })
                    .Subscribe();
                await bot.Start();
                await bot.Completion;
            }
        }

        private static void WriteEvent(IClientEvent @event)
        {
            Log.Debug("{@Event}", @event);
        }
    }
}
