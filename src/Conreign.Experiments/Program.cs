using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Conreign.Client.Contracts;
using Conreign.Client.Contracts.Messages;
using Conreign.Client.Handler;
using Conreign.Client.Orleans;
using Conreign.Client.SignalR;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Battle.AI;
using Conreign.LoadTest.Core;
using Conreign.LoadTest.Core.Behaviours;
using Conreign.Server.Contracts.Communication;
using Orleans.Runtime.Configuration;
using Serilog;
using SimpleInjector;

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
            TestHandler().Wait();
            //TestSignalR().Wait();
            Console.WriteLine("Press a key to exit...");
            Console.ReadLine();
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

        private static async Task TestSignalR()
        {
            try
            {
                var options = new SignalRClientOptions("http://localhost:9000")
                {
                    IsDebug = false
                };
                var client = new SignalRClient(options);
                using (var connection = await client.Connect(Guid.NewGuid()))
                {
                    var loginResult = await connection.Login();
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "SignalR client error: {Message}", ex.Message);
            }
        }

        private static async Task TestHandler()
        {
            var config = ClientConfiguration.LocalhostSilo();
            config.AddSimpleMessageStreamProvider(StreamConstants.ProviderName);
            var host = new OrleansClientInitializer(config);
            var client = await OrleansClient.Initialize(host);
            var container = new Container();
            container.RegisterClientHandlerFactory();
            var handlerFactory = container.GetInstance<ClientHandlerFactory>();
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                var handler = handlerFactory.Create(connection);
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
                var write = new SendMessageCommand
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
            var options = new SignalRClientOptions("http://localhost")
            {
                IsDebug = false
            };
            var client = new SignalRClient(options);
            using (var connection = await client.Connect(Guid.NewGuid()))
            {
                await RunBot(connection, roomId, i, total);
            }
        }

        private static async Task RunOrleansBot(string roomId, int i, int total)
        {
            var config = ClientConfiguration.LoadFromFile("OrleansClientConfiguration.xml");
            var host = new OrleansClientInitializer(config);
            var client = await OrleansClient.Initialize(host);
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
            var options = new RankingBotBattleStrategyOptions(0.8, 0.2, 1);
            var behaviours = new List<IBotBehaviour>
            {
                new LoginBehaviour(),
                new JoinRoomBehaviour(roomId, isLeader ? TimeSpan.Zero : TimeSpan.FromSeconds(0.5)),
                new BattleBehaviour(new RankingBotBattleStrategy(options)),
                new StopOnGameEndBehaviour()
            };
            if (isLeader)
            {
                behaviours.Add(new StartGameBehaviour(total));
            }
            using (var bot = new Bot(name, connection, behaviours.ToArray()))
            {
                await bot.Run();
            }
        }

        private static void Write(object data)
        {
            Log.Debug("{@Data}", data);
        }
    }
}