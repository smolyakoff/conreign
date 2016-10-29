using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Conreign.Client.Handler;
using Conreign.Client.Orleans;
using Conreign.Client.SignalR;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Gameplay.AI;
using Conreign.Core.Gameplay.AI.Battle;
using Conreign.Core.Gameplay.AI.Behaviours;
using Serilog;

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
            Simulate();
            //TestHandler();
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
            const int players = 5;
            var tasks = Enumerable.Range(0, rooms)
                .Select(i => $"conreign-bots-{i}")
                .SelectMany((roomId, i) => Enumerable.Range(0, players).Select(k => RunSignalRBot(roomId, k, players)))
                .ToArray();
            Task.WaitAll(tasks);
        }

        private static async Task RunHandler()
        {
            var client = await OrleansClient.Initialize("OrleansClientConfiguration.xml");
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                var handler = new ClientHandler(connection);
                handler.Events.Subscribe(Write);
                var meta = new Metadata();
                var loginResponse = await handler.Handle(new LoginCommand(), meta);
                Write(loginResponse);
                meta = new Metadata {AccessToken = loginResponse.AccessToken};
                var joinRoom = new JoinRoomCommand {RoomId = "conreign"};
                await handler.Handle(joinRoom, meta);
                var updatePlayer = new UpdatePlayerOptionsCommand
                {
                    RoomId = "conreign",
                    Options = new PlayerOptionsData
                    {
                        Nickname = "smolyakoff",
                        Color = "#010101"
                    }
                };
                await handler.Handle(updatePlayer, meta);
                var write = new WriteCommand
                {
                    RoomId = "conreign",
                    Text = "Hello!"
                };
                await handler.Handle(write, meta);
                var getState = new GetRoomStateCommand
                {
                    RoomId = "conreign"
                };
                var state = await handler.Handle(getState, meta);
                Write(state);
            }
        }

        private static async Task RunSignalRBot(string roomId, int i, int total)
        {
            ServicePointManager.DefaultConnectionLimit = 500;
            var options = new SignalRClientOptions {ConnectionUri = "http://localhost:9000"};
            var client = new SignalRClient(options);
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                await RunBot(connection, roomId, i, total);
            }
        }

        private static async Task RunOrleansBot(string roomId, int i, int total)
        {
            var client = await OrleansClient.Initialize("OrleansClientConfiguration.xml");
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                await RunBot(connection, roomId, i, total);
            }
        }

        private static async Task RunBot(IClientConnection connection, string roomId, int i, int total)
        {
            var isLeader = i == 0;
            var name = isLeader ? "Leader" : $"Bot-{i}";
            Console.WriteLine($"Connection id: {connection.Id}");
            var options = new NaiveBotBattleStrategyOptions(0.8, 0.2, 1);
            var behaviours = new List<IBotBehaviour>
            {
                new LoginBehaviour(),
                new LogBehaviour(),
                new JoinRoomBehaviour(roomId, name, isLeader ? TimeSpan.Zero : TimeSpan.FromSeconds(0.5)),
                new BattleBehaviour(new NaiveBotBattleStrategy(options)),
                new StopOnGameEndBehaviour()
            };
            if (isLeader)
            {
                behaviours.Add(new StartGameBehaviour(total));
            }
            using (var bot = new Bot(name, connection, behaviours.ToArray()))
            {
                bot.Start();
                await bot.Completion;
            }
        }

        private static void Write(object data)
        {
            Log.Debug("{@Data}", data);
        }
    }
}