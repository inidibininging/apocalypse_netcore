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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics;

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
        IGameSectorsOwner, IWorldGame
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

        public GameServerConfiguration Configuration { get; set; }

        #endregion Server stuff
        
        #region EntityFactories

        public PlayerSpaceshipFactory PlayerFactory { get; set; } = new PlayerSpaceshipFactory();
        private ISerializationAdapter SerializationAdapter { get; set; }
        #endregion EntityFactories

        public WorldGame
        (
            GameServerConfiguration configuration
        )
        {
            Configuration = configuration;
            var leSerializationType = configuration.SerializationAdapterType.LoadType(true, false)[0];
            SerializationAdapter = Activator.CreateInstance(leSerializationType) as ISerializationAdapter;
            GameSectorLayerServices = new Dictionary<int, IStateMachine<string, IGameSectorLayerService>>();
            SectorStateMachine = new InMemoryStorageGameSectorLayerServiceFactory();
            AuthenticationService = new ExampleLoginAndRegistrationService();
            CreateServer(
                Configuration.ServerPeerName,
                Configuration.ServerIp,
                Configuration.ServerPort);

            //translator
            var serverTranslator = new NetworkCommandTranslator(SerializationAdapter);
            var serverMessageTranslator = new NetIncomingMessageNetworkCommandConnectionTranslator(serverTranslator);


            var cliPassthrough = new CLIPassthroughMechanic(AuthenticationService, Configuration.RunOperation);
            var writer = new GameSectorLayerWriterMechanic
            {
                RedisHost = Configuration.RedisHost,
                RedisPort = Configuration.RedisPort
            };
            var rediCliPassthrough = new RedisCLIPassthroughMechanic(AuthenticationService)
            {
                RedisHost = Configuration.RedisHost,
                RedisPort = Configuration.RedisPort
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

            var inGameSectorStateMachine = GameSectorLayerServices[Configuration.StartingSector];
            inGameSectorStateMachine.SharedContext = new GameSectorLayerService
            {
                Tag = Configuration.StartingSector
            };
            
            inGameSectorStateMachine
                .GetService
                .Get(Configuration.BuildOperation)
                .Handle(inGameSectorStateMachine);
            

            //datalayer to gather data out of the game world
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

            //Language script file
            LanguageScriptFileEvaluator languageScriptFileEvaluator = new LanguageScriptFileEvaluator(Configuration.StartupScript, Configuration.StartupFunction, Configuration.RunOperation);
            foreach (var sector in GameSectorLayerServices.Values)
            {
                languageScriptFileEvaluator.Evaluate(sector);
            }

            Console.WriteLine("runner starting..." + Configuration.StartupFunction);
            CreateGameTimeIfNotExists(null);

            foreach (var sector in GameSectorLayerServices.Values)
            {
                sector.SharedContext.CurrentGameTime = CurrentGameTime;
                sector.Run(Configuration.StartupFunction);
            }

            //Set starting sector to run
            GameSectorLayerServices[Configuration.StartingSector]
                .GetService
                .Get("MarkSectorAsRunning")
                .Handle(GameSectorLayerServices[Configuration.StartingSector]);
        }

        private void BuildGrid()
        {
            const int columnCount = 2;
            for (int cell = 0; cell < 2; cell++)
            {
                var nextCell = cell % columnCount > 0 ? Math.Abs(cell - 1) : cell + 1;
                BuildSector(
                    cell,
                    Math.Abs(cell - columnCount),
                    nextCell,
                    nextCell,
                    cell + columnCount); ;
            }
        }
         

        private void BuildSector(int sectorName,
            int sectorUp,
            int sectorLeft,
            int sectorRight,
            int sectorDown)
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

            var playerShifter = new RouterPlayerShifterMechanic(
                new RouteDualMediator(null, null),
                new List<GameSectorRoutePair>() {
                    CreateRoutePair(GameSectorTrespassingDirection.Down,sectorName, sectorDown),
                    CreateRoutePair(GameSectorTrespassingDirection.Left,sectorName, sectorLeft),
                    CreateRoutePair(GameSectorTrespassingDirection.Right,sectorName, sectorRight),
                    CreateRoutePair(GameSectorTrespassingDirection.Up,sectorName, sectorUp),
                });
            var routeTrespassingMarker = new RouteTrespassingMarkerMechanic(new RouteDualMediator(null, null), 100);
            var mediator = new RouteDualMediator(routeTrespassingMarker, playerShifter);
            routeTrespassingMarker.setRouteMediator(mediator);
            playerShifter.SetRouteMediator(mediator);

            SectorsOwnerMechanics.Add(routeTrespassingMarker);
            SectorsOwnerMechanics.Add(playerShifter);
        }

        private static GameSectorRoutePair CreateRoutePair(GameSectorTrespassingDirection trespassingDirection, int sourceSector, int destinationSector)
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

            GameSectorLayerServices.Add(sectorId, SectorStateMachine.BuildStateMachine(Configuration));
        }

        private TimeSpan TotalRealTime { get; set; }
        private TimeSpan TotalGameTime { get; set; }

        private GameTime CurrentGameTime { get; set; }

        private GameTime CreateGameTime()
        {
            TotalGameTime = TimeSpan.FromSeconds(Configuration.ServerUpdateInSeconds);
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
            GameStateContext.Update();

            foreach (var sectorMechanic in SectorsOwnerMechanics)
            {
                sectorMechanic.Update(this);
            }
            //foreach (var sectorOwnerMechanic in SectorsOwnerMechanics)       
            
            foreach (var sector in GameSectorLayerServices.Values.Where(sector => sector.SharedContext.CurrentStatus == GameSectorStatus.Running))
            {
                sector.SharedContext.CurrentGameTime = CurrentGameTime;
                sector
                    .SharedContext
                    .EventDispatcher
                    .DispatchEvents(gameTime);
                sector
                    .GetService
                    .Get(Configuration.RunOperation)
                    .Handle(sector);
                
            }
            Thread.Sleep(TimeSpan.FromSeconds(Configuration.ServerUpdateInSeconds));
        }

        private PlayerSpaceship CreatePlayerSpaceShip(string loginToken)
        {
            return PlayerFactory.Create(loginToken);
        }

        public GameStateData GetGameStateByLoginToken(string loginToken)
        {
            //TODO: proxy ?? pattern 
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
                var problematicGameState = foundGameStates.GroupBy(gs => gs.sectorTag).Where(gameStates => gameStates.Count() > 1).Select(gs => gs.Key);
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
            if (user != null)
            {
                var userGameStatedata = RegisterGameStateData(loginToken);
                if ((user.Roles & UserDataRole.CanSendRemoteStateCommands) != 0)
                {
                    //offer the player remote control on the server :) ... or :(
                    userGameStatedata.Metadata = new IdentifiableNetworkCommand() { CommandName = CLINetworkCommandConstants.WaitForSignalCommand };
                }
            }
            throw new NotImplementedException("new users cannot be inserted into this demo");
        }



        public GameStateData RegisterGameStateData(string loginToken)
        {
            Console.WriteLine($"{nameof(RegisterGameStateData)} in World");
            var newPlayer = CreatePlayerSpaceShip(loginToken);
            newPlayer.CurrentImage.Position.X = GameSectorLayerServices[Configuration.StartingSector].SharedContext.SectorBoundaries.MaxSectorX / 2;
            newPlayer.CurrentImage.Position.Y = GameSectorLayerServices[Configuration.StartingSector].SharedContext.SectorBoundaries.MaxSectorY / 2;

            GameSectorLayerServices[Configuration.StartingSector]
                    .SharedContext
                    .DataLayer
                    .Players.Add(newPlayer);
            
            FirePlayerRegisteredEvent(newPlayer);

            //for demo purpouses
            var demoItem = GameSectorLayerServices[Configuration.StartingSector]
                            .SharedContext
                            .Factories
                            .ItemFactory
                            [nameof(MockItemFactory)]
                            .Create(GameSectorLayerServices[Configuration.StartingSector]
                            .SharedContext.SectorBoundaries);
            if (demoItem != null)
            {
                demoItem.OwnerName = newPlayer.DisplayName;
                GameSectorLayerServices[Configuration.StartingSector]
                    .SharedContext
                    .DataLayer
                    .Items
                    .Add(demoItem);
            }

            return GameSectorLayerServices[Configuration.StartingSector]
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
            var playerRegisteredEventLayer = GameSectorLayerServices[Configuration.StartingSector]
                .SharedContext
                .DataLayer
                .Layers
                .FirstOrDefault(l => l.DisplayName == PlayerRegisteredEventName &&
                                     l.GetValidTypes().Any(t => t == typeof(EventQueueArgument)));
            if(playerRegisteredEventLayer == null)
            {
                throw new InvalidOperationException($"Cannot use {nameof(FirePlayerRegisteredEvent)}. EventQueue PlayerRegistered doesn't exist");
            }

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
