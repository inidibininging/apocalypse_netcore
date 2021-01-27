using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Network.Server.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.GameServer.Services;
using Apocalypse.Any.GameServer.States.Sector;
using Apocalypse.Any.GameServer.States.Sector.Storage;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Data;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.YamlAdapter;
using Apocalypse.Any.Infrastructure.Server.Adapters.Redis;
using Apocalypse.Any.Infrastructure.Server.Language;
using Apocalypse.Any.Infrastructure.Server.PubSub;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.CLI;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.RoutingMechanics;
using Apocalypse.Any.Infrastructure.Server.States;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
using Serilog;
using Serilog.Extensions.Logging;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics;
using Apocalypse.Any.Infrastructure.Server.Worker;
using Apocalypse.Any.Core.Input.Translator;

namespace Apocalypse.Any.GameServer.GameInstance
{
    /// <summary>
    /// This is a wrapper around server game objects
    /// with a list of state machines of sectors
    /// </summary>
    public class WorldGame :
        IUpdateableLite,
        IWorldGameStateDataIOLayer,
        IWorldGameSectorInputLayer,
        IGameSectorsOwner,
        IWorldGame
    {
        //private Dictionary<string, IGameSectorNew> Sectors { get; set; }

        /// <summary>
        /// Sectors contexts.
        /// It means you dont return game sectors but rather a context to a game sector.
        /// </summary>
        public Dictionary<int, IStateMachine<string, IGameSectorLayerService>> GameSectorLayerServices { get; set; }

        /// <summary>
        /// Factory for game sector contexts
        /// </summary>
        public IGameSectorLayerServiceStateMachineFactory<GameServerConfiguration> SectorStateMachine { get; set; }

        public IList<ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>> SectorsOwnerMechanics { get; set; }

        #region Server stuff

        /// <summary>
        /// Server has a server context.
        /// </summary>
        private ServerNetworkGameStateContext<WorldGame> GameStateContext { get; set; }

        private NetServer Server { get; set; }


        private NetIncomingMessageBusService<NetServer> ServerInput { get; set; }
        private NetOutgoingMessageBusService<NetServer> ServerOutput { get; set; }

        private ExampleLoginAndRegistrationService AuthenticationService { get; set; }

        public GameServerConfiguration ServerConfiguration { get; set; }

        #endregion Server stuff

        #region EntityFactories

        public PlayerSpaceshipFactory PlayerFactory { get; set; } = new PlayerSpaceshipFactory();
        private IByteArraySerializationAdapter SerializationAdapter { get; }

        private GameClientConfiguration ClientConfiguration { get; }
        #endregion EntityFactories

        #region SyncClient
        private SyncClient<PlayerSpaceship,EnemySpaceship,Item,Projectile,CharacterEntity,CharacterEntity,ImageData> SendPressReleaseWorker { get; }
        private bool LoggedInToPressRelease { get; set; }
        private IntCommandStringCommandTranslator SyncCommandTranslator { get; } = new IntCommandStringCommandTranslator();
        #endregion

        public WorldGame(GameServerConfiguration serverConfiguration, GameClientConfiguration clientConfiguration)
        {
            ServerConfiguration = serverConfiguration;
            ClientConfiguration = clientConfiguration;

            var leSerializationType = serverConfiguration.SerializationAdapterType.LoadType(false, false);
            if (leSerializationType == null || leSerializationType.Length == 0)
                throw new Exception("Serializer cannot be loaded");

            var serializerType = leSerializationType.FirstOrDefault() ?? throw new Exception($"Cannot load serializer type {serverConfiguration.SerializationAdapterType}");

            SerializationAdapter = Activator.CreateInstance(serializerType) as IByteArraySerializationAdapter;
            GameSectorLayerServices = new Dictionary<int, IStateMachine<string, IGameSectorLayerService>>();
            SectorStateMachine = new InMemoryStorageGameSectorLayerServiceFactory();
            AuthenticationService = new ExampleLoginAndRegistrationService();
            CreateServer(
                ServerConfiguration.ServerPeerName,
                ServerConfiguration.ServerIp,
                ServerConfiguration.ServerPort);

            //translator
            var serverTranslator = new NetworkCommandTranslator(SerializationAdapter);
            var serverMessageTranslator = new NetIncomingMessageNetworkCommandConnectionTranslator(serverTranslator);
            var cliPassthrough = new CLIPassthroughMechanic(AuthenticationService, ServerConfiguration.RunOperation);
            var writer = new GameSectorLayerWriterMechanic
            {
                RedisHost = ServerConfiguration.RedisHost,
                RedisPort = ServerConfiguration.RedisPort
            };
            var rediCliPassthrough = new RedisCLIPassthroughMechanic(AuthenticationService)
            {
                RedisHost = ServerConfiguration.RedisHost,
                RedisPort = ServerConfiguration.RedisPort
            };
            
            var transferStuff = new TransferPlayerStuffBetweenSectorsMechanic();
            var updateSectorStatusMechanic = new UpdateSectorStatusMechanic();
            SectorsOwnerMechanics = new List<ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>>
            {
                //routeTrespassingMarker,
                //playerShifter,
                cliPassthrough,
                rediCliPassthrough,
                transferStuff,
                updateSectorStatusMechanic,
                writer
            };

            //create default starting sector    
            BuildGrid();

            var inGameSectorStateMachine = GameSectorLayerServices[ServerConfiguration.StartingSector];
            inGameSectorStateMachine.SharedContext = new GameSectorLayerService
            {
                Tag = ServerConfiguration.StartingSector
            };

            inGameSectorStateMachine
                .GetService
                .Get(ServerConfiguration.BuildOperation)
                .Handle(inGameSectorStateMachine);

            var serverStateDataLayer = new ServerGameStateService<WorldGame>(
                                                AuthenticationService,
                                                AuthenticationService,
                                                this,
                                                SerializationAdapter);

            GameStateContext = new ServerNetworkGameStateContext<WorldGame>(
                ServerInput,
                ServerOutput,
                serverMessageTranslator,
                serverStateDataLayer,
                GetLogger());

            GameStateContext.Initialize();

            //Connection to the real server
            if(ClientConfiguration != null)
            {
                SendPressReleaseWorker =
                    new SyncClient<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity,ImageData>(ClientConfiguration);
            }

            //Language script file
            LanguageScriptFileEvaluator languageScriptFileEvaluator = new LanguageScriptFileEvaluator(ServerConfiguration.StartupScript, ServerConfiguration.StartupFunction, ServerConfiguration.RunOperation);
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var sector in GameSectorLayerServices.Values)
            {
                languageScriptFileEvaluator.Evaluate(sector);
                Console.Write(".");
            }
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("runner starting..." + ServerConfiguration.StartupFunction);
            CreateGameTimeIfNotExists(null);

            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var sector in GameSectorLayerServices.Values)
            {
                sector.SharedContext.CurrentGameTime = CurrentGameTime;
                sector.Run(ServerConfiguration.StartupFunction);
                Console.Write(".");
            }
            Console.ForegroundColor = ConsoleColor.White;

            //Set starting sector to run
            Console.WriteLine($"Done loading sectors. Starting sector {ServerConfiguration.StartingSector}");
            GameSectorLayerServices[ServerConfiguration.StartingSector]
                .GetService
                .Get("MarkSectorAsRunning")
                .Handle(GameSectorLayerServices[ServerConfiguration.StartingSector]);
        }

        /// <summary>
        /// Builds a looping grid out of sectors. By default is 4x4 = 16 Sectors
        /// </summary>
        private void BuildGrid(int size = 4)
        {
            var columnCount = size;
            var rowCount = size;
            var cell = 1;
            var gridTrespassingMechanic = new RouteTrespassingMarkerMechanic();

            for (int y = 1; y <= columnCount; y++)
            {
                for (int x = 1; x <= rowCount; x++)
                {
                    var left = -1;
                    var right = 1;
                    var up = columnCount * -1;
                    var down = columnCount;

                    if (y == 1)
                    {
                        up = ((rowCount * columnCount) - (rowCount));
                    }

                    if (y == columnCount)
                    {
                        down = ((rowCount * columnCount) - (rowCount)) * -1;
                    }

                    if (x == 1)
                    {
                        left = rowCount - 1;
                    }

                    if (x == rowCount)
                    {
                        right = (rowCount - 1) * -1;
                    }

                    Console.ForegroundColor = ConsoleColor.Red;
                    BuildSector(cell);
                    Console.Write(".");
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"x:{x} y:{y} c:{cell} u:{cell + up} l:{cell + left} r:{cell + right} d:{cell + down}");

                    gridTrespassingMechanic.RegisterRoutePair(
                        CreateRoutePair(GameSectorTrespassingDirection.Up, cell, cell + up));
                    gridTrespassingMechanic.RegisterRoutePair(
                        CreateRoutePair(GameSectorTrespassingDirection.Left, cell, cell + left));
                    gridTrespassingMechanic.RegisterRoutePair(
                        CreateRoutePair(GameSectorTrespassingDirection.Right, cell, cell + right));
                    gridTrespassingMechanic.RegisterRoutePair(
                        CreateRoutePair(GameSectorTrespassingDirection.Down, cell, cell + left));

                    cell += 1;
                }

            }

            SectorsOwnerMechanics.Add(gridTrespassingMechanic);
        }


        private void BuildSector(int sectorName)
        {
            //Factory has to build this
            AddSectorStateMachine(sectorName);
            var sourceSector = GameSectorLayerServices[sectorName];
            sourceSector.SharedContext = new GameSectorLayerService
            {
                Tag = sectorName
            };

            sourceSector
                .GetService
                .Get(ServerGameSectorNewBook.BuildDefaultSectorState)
                .Handle(sourceSector);

        }

        private GameSectorRoutePair CreateRoutePair(GameSectorTrespassingDirection trespassingDirection, int sourceSector, int destinationSector)
        {
            return new GameSectorRoutePair()
            {
                Trespassing = trespassingDirection,
                GameSectorTag = sourceSector,
                GameSectorDestinationTag = destinationSector,
            };
        }

        /// <summary>
        /// Creates a server object ( NetServer ) for interacting with clients
        /// </summary>
        /// <param name="serverPeerName">the peer name of the server</param>
        /// <param name="serverIp">duh the ip OR what is not so plausible (the host name)</param>
        /// <param name="serverPort">the server port</param>
        private void CreateServer(string serverPeerName, string serverIp, int serverPort)
        {
            //net incoming message bus service creation
            var serverPeerConfig = new NetPeerConfiguration(serverPeerName)
            {
                Port = serverPort,
                AcceptIncomingConnections = true,
                EnableUPnP = true,
                LocalAddress = IPAddress.Parse(serverIp)
            };
            Server = new NetServer(serverPeerConfig);
            ServerInput = new NetIncomingMessageBusService<NetServer>(Server);
            ServerOutput = new NetOutgoingMessageBusService<NetServer>(Server, SerializationAdapter);
            Server.Start();
            Console.WriteLine(Server.ToString());
        }

        private void AddSectorStateMachine(int sectorId)
        {
            if (GameSectorLayerServices.ContainsKey(sectorId))
                throw new SectorAlreadyExistsException(sectorId);

            GameSectorLayerServices.Add(sectorId, SectorStateMachine.BuildStateMachine(ServerConfiguration));
        }

        private TimeSpan TotalRealTime { get; set; }
        private TimeSpan TotalGameTime { get; set; }

        private GameTime CurrentGameTime { get; set; }

        private GameTime CreateGameTime()
        {
            TotalGameTime = TimeSpan.FromSeconds(ServerConfiguration.ServerUpdateInSeconds);
            TotalRealTime = TimeSpan.FromTicks(0);
            return new GameTime(TotalGameTime, TotalRealTime);
        }

        private void UpdateGameTime(GameTime gameTime)
        {
            if (gameTime is null)
            {
                throw new ArgumentNullException(nameof(gameTime));
            }

            TotalRealTime = TotalRealTime.Add(TimeSpan.FromTicks(TotalGameTime.Ticks));
            CurrentGameTime = new GameTime(TotalGameTime, TotalRealTime);
        }
        private void CreateGameTimeIfNotExists(GameTime gameTime)
        {
            if (gameTime == null && CurrentGameTime == null)
                CurrentGameTime = CreateGameTime();
        }
        public void Update(GameTime gameTime)
        {
            CreateGameTimeIfNotExists(gameTime);
            UpdateGameTime(CurrentGameTime);

            //Try to login to the "real server"
            if(!LoggedInToPressRelease)
                LoggedInToPressRelease = (SendPressReleaseWorker?.TryLogin()).GetValueOrDefault();


            GameStateContext.Update();

            foreach (var sectorMechanic in SectorsOwnerMechanics)
            {
                sectorMechanic.Update(this);
            }

            var timeToWait = TimeSpan.FromSeconds(ServerConfiguration.ServerUpdateInSeconds);

            foreach (var sector in GameSectorLayerServices
                                                                                    .Values
                                                                                    .Where(sector => sector.SharedContext.CurrentStatus == GameSectorStatus.Running && sector.SharedContext.DataLayer.Players.Any()))
            {
                sector.SharedContext.CurrentGameTime = CurrentGameTime;
                    Task.Factory.StartNew(() =>
                    {
                        sector
                            .SharedContext
                            .EventDispatcher
                            .DispatchEvents(gameTime);
                        sector
                            .GetService
                            .Get(ServerConfiguration.RunOperation)
                            .Handle(sector);
                    });
            }

            //Sync server logic
            if(LoggedInToPressRelease && SendPressReleaseWorker != null){
                var gameStateLoginToken = GetGameStateByLoginToken(SendPressReleaseWorker.LoginToken);
                SendPressReleaseWorker?.ProcessIncomingMessages(gameStateLoginToken.Commands.Select(cmd => SyncCommandTranslator.Translate(cmd)));
            }

            Thread.Sleep(timeToWait);
        }

        private PlayerSpaceship CreatePlayerSpaceShip(string loginToken)
        {
            return PlayerFactory.Create(loginToken);
        }

        public GameStateData GetGameStateByLoginToken(string loginToken)
        {
            var foundGameStates = GameSectorLayerServices
                                .Values
                                .Select((sector) =>
                                {
                                    try
                                    {
                                        return (gameState: sector
                                                            .SharedContext
                                                            .IODataLayer
                                                            .GetGameStateByLoginToken(loginToken), sectorTag: sector.SharedContext.Tag);
                                    }
                                    catch (Exception ex)
                                    {
                                        sector.SharedContext.Messages.Add(ex.Message);
                                        return (gameState: null, sectorTag: 0);
                                    }
                                });

            foundGameStates = foundGameStates.Where(gameState => gameState.gameState != null);

            if (foundGameStates.Any())
            {
                var problematicGameState = foundGameStates
                                                                .GroupBy(gs => gs.sectorTag)
                                                                .Where(gameStates => gameStates.Count() > 1)
                                                                .Select(gs => gs.Key);
                if(problematicGameState.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"Sectors {string.Join(',', problematicGameState.Select(gs => gs.ToString()))} have more than one gamestate");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                return foundGameStates.First().gameState;
            }

            Console.WriteLine($"{nameof(GetGameStateByLoginToken)}:Found NO game state ");
            var user = AuthenticationService.GetByLoginTokenHack(loginToken);
            if (user == null) throw new NotImplementedException("new users cannot be inserted into this demo");
            var userGameStateData = RegisterGameStateData(loginToken);
            if ((user.Roles & UserDataRole.CanSendRemoteStateCommands) != 0)
            {
                //offer the player remote control on the server :) ... or :(
                userGameStateData.Metadata = new IdentifiableNetworkCommand() { CommandName = CLINetworkCommandConstants.WaitForSignalCommand };
            }
            return userGameStateData;
        }

        public GameStateData RegisterGameStateData(string loginToken)
        {
            Console.WriteLine($"{nameof(RegisterGameStateData)} in World");
            var newPlayer = CreatePlayerSpaceShip(loginToken);
            newPlayer.CurrentImage.Position.X = GameSectorLayerServices[ServerConfiguration.StartingSector].SharedContext.SectorBoundaries.MaxSectorX / 2;
            newPlayer.CurrentImage.Position.Y = GameSectorLayerServices[ServerConfiguration.StartingSector].SharedContext.SectorBoundaries.MaxSectorY / 2;

            GameSectorLayerServices[ServerConfiguration.StartingSector]
                    .SharedContext
                    .DataLayer
                    .Players.Add(newPlayer);

            FirePlayerRegisteredEvent(newPlayer);

            return GameSectorLayerServices[ServerConfiguration.StartingSector]
                    .SharedContext
                    .IODataLayer
                    .RegisterGameStateData(loginToken);
        }

        /// <summary>
        /// Fires an event if the player is registered for the first time
        /// </summary>
        /// <param name="newPlayer"></param>
        private void FirePlayerRegisteredEvent(PlayerSpaceship newPlayer)
        {
            const string PlayerRegisteredEventName = "PlayerRegisteredEvent";
            var playerRegisteredEventLayer = GameSectorLayerServices[ServerConfiguration.StartingSector]
                .SharedContext
                .DataLayer
                .Layers
                .FirstOrDefault(l => l.DisplayName == PlayerRegisteredEventName &&
                                     l.GetValidTypes().Any(t => t == typeof(EventQueueArgument)));
            if(playerRegisteredEventLayer == null)
            {
                throw new InvalidOperationException($"Cannot use {nameof(FirePlayerRegisteredEvent)}. EventQueue PlayerRegistered doesn't exist");
            }
	    
	        //this needs to be created through a factory
            var playerRegisteredEvent = new EventQueueArgument()
            {
                Id = Guid.NewGuid().ToString(),
                EventName = PlayerRegisteredEventName,
                ReferenceId = newPlayer.Id, //playerRegisteredEventRelation.Id
                ReferenceType = typeof(PlayerSpaceship)
            };

            //TODO: event queue argument factory
            playerRegisteredEventLayer.Add(playerRegisteredEvent);
        }

        private TimeSpan SendingDelta { get; set; } = TimeSpan.Zero;
        public bool ForwardClientDataToGame(GameStateUpdateData updateData)
        {
            //build it
            var now = DateTime.Now;
            return GameSectorLayerServices
                .Values
                .ToList()
                .Any(sector =>
            {
                var currentPlayer = sector
                                    .SharedContext
                                    .DataLayer
                                    .Players
                                    .FirstOrDefault(player => player.LoginToken == updateData.LoginToken);


                if (currentPlayer == null)
                    return false;

                var sent = sector
                .SharedContext
                .IODataLayer
                .ForwardClientDataToGame(updateData);
                SendingDelta = DateTime.Now - now;
                return sent;
            });
        }

        public bool ForwardServerDataToGame(GameStateData gameStateData)
        {
            //build it
            return GameSectorLayerServices
                .Values
                .ToList()
                .Any(sector =>
                {
                    var currentPlayer = sector
                                        .SharedContext
                                        .DataLayer
                                        .Players
                                        .Where(player => player.LoginToken == gameStateData.LoginToken);

                    if (!currentPlayer.Any())
                        return false;

                    if (currentPlayer.Count() > 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("Something is wrong ... player with same login token found in different sectors");
                        Console.ForegroundColor = ConsoleColor.White;
                    }

                    return sector
                    .SharedContext
                    .IODataLayer
                    .ForwardServerDataToGame(gameStateData);
                });
        }

        public void BroadcastMessage(string message)
        {
            foreach (var sector in this.GameSectorLayerServices.Values)
                sector.SharedContext.Messages.Add(message);
        }

        public IGameSectorLayerService GetSector(int sectorIdentifier) => GameSectorLayerServices[sectorIdentifier].SharedContext;

        private ILogger<byte> GetLogger()
        {
            var providers = new LoggerProviderCollection();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.ColoredConsole()
                .WriteTo.Providers(providers)
                .CreateLogger();

            var services = new ServiceCollection();

            services.AddSingleton(providers);
            services.AddSingleton<ILoggerFactory>(sc =>
            {
                var providerCollection = sc.GetService<LoggerProviderCollection>();
                var factory = new SerilogLoggerFactory(null, true, providerCollection);

                foreach (var provider in sc.GetServices<ILoggerProvider>())
                    factory.AddProvider(provider);

                return factory;
            });

            services.AddLogging(l => l.AddConsole());

            var serviceProvider = services.BuildServiceProvider();
            var logger = serviceProvider.GetRequiredService<ILogger<byte>>();

            return logger;
        }
    }
}
