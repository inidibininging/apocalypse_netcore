using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Network.Server.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Configuration.Model;
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
        public Dictionary<string, IStateMachine<string, IGameSectorLayerService>> GameSectorLayerServices { get; set; }

        /// <summary>
        /// Factory for game sector contexts
        /// </summary>
        public IGameSectorLayerServiceStateMachineFactory<IGameSectorData> SectorStateMachine { get; set; }

        public IList<ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>> SectorsOwnerMechanics { get; set; }

        #region Server stuff

        /// <summary>
        /// Server has a server context.
        /// </summary>
        private ServerNetworkGameStateContext<WorldGame> GameStateContext { get; set; }

        private NetServer Server { get; set; }
        private NetIncomingMessageBusService<NetServer> ServerInput { get; set; }
        private NetOutgoingMessageBusService<NetServer> ServerOutput { get; set; }

        private IUserAuthenticationService AuthenticationService { get; set; }

        public GameServerConfiguration Configuration { get; set; }

        #endregion Server stuff

        //private string StartingSector { get; } = "hub";

        #region EntityFactories

        public PlayerSpaceshipFactory PlayerFactory { get; set; } = new PlayerSpaceshipFactory();
        private ISerializationAdapter SerializationAdapter { get; set; }
        #endregion EntityFactories

        public WorldGame(GameServerConfiguration configuration)
        {
            Configuration = configuration;

            var leSerializationType = configuration.SerializationAdapterType.LoadType(true, false)[0];
            SerializationAdapter = Activator.CreateInstance(leSerializationType) as ISerializationAdapter;
            
            GameSectorLayerServices = new Dictionary<string, IStateMachine<string, IGameSectorLayerService>>();
            AuthenticationService = new ExampleLoginAndRegistrationService();
            SectorStateMachine = new InMemoryStorageGameSectorLayerServiceFactory(SerializationAdapter);
            
            CreateServer(Configuration.ServerPeerName, Configuration.ServerIp, Configuration.ServerPort);
            CreateWorldLayersAndMechanics();
            CreateServerCommunicationLayer();

            var startingSector = Configuration.SectorConfigurations.FirstOrDefault(s => s.Tag == Configuration.StartingSector);
            //create default starting sector            
            BuildSector(
                Configuration.StartingSector,
                Configuration.StartingSector,
                Configuration.StartingSector,
                Configuration.StartingSector,
                Configuration.StartingSector);


            //MAIN SECTOR
            var inGameSectorStateMachine = GameSectorLayerServices[Configuration.StartingSector];
            inGameSectorStateMachine.SharedContext = new GameSectorLayerService
            {
                Tag = Configuration.StartingSector
            };

            //inGameSectorStateMachine.SharedContext.CurrentStatus = GameSectorStatus.Running;
            inGameSectorStateMachine
                .GetService
                .Get(startingSector.BuildOperation)
                .Handle(inGameSectorStateMachine);

            var sectorList = new List<string>();
            for (int sectorIndex = 0; sectorIndex < 1; sectorIndex++)
            {
                var sectorId = System.Guid.NewGuid().ToString();

                var sectorDown = Configuration.StartingSector;
                var sectorUp = Configuration.StartingSector;
                var sectorLeft = Configuration.StartingSector;
                var sectorRight = Configuration.StartingSector;

                if (sectorIndex - 4 > 0)
                    sectorDown = sectorList[sectorIndex - 4];
                if (sectorIndex - 3 > 0)
                    sectorUp = sectorList[sectorIndex - 3];
                if (sectorIndex - 2 > 0)
                    sectorLeft = sectorList[sectorIndex - 2];
                if (sectorIndex - 1 > 0)
                    sectorRight = sectorList[sectorIndex - 1];

                sectorList.Add(sectorId);
                BuildSector(sectorId, sectorUp, sectorLeft, sectorRight, sectorDown);
            }



            //Language script file
            LanguageScriptFileEvaluator languageScriptFileEvaluator = new LanguageScriptFileEvaluator(startingSector.StartupScript, startingSector.StartupFunction);
            foreach (var sector in GameSectorLayerServices.Values)
            {
                languageScriptFileEvaluator.Evaluate(sector);
            }

            Console.WriteLine("runner starting..." + startingSector.StartupFunction);
            CreateGameTimeIfNotExists(null);
            Console.WriteLine(CurrentGameTime);
            foreach (var sector in GameSectorLayerServices.Values)
            {
                sector.SharedContext.CurrentGameTime = CurrentGameTime;
                sector.Run(startingSector.StartupFunction);
            }
        }

        private void CreateWorldLayersAndMechanics()
        {
            var cliPassthrough = new CLIPassthroughMechanic(AuthenticationService);
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

            SectorsOwnerMechanics = new List<ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>>
            {
                //routeTrespassingMarker,
                //playerShifter,
                cliPassthrough,
                rediCliPassthrough,
                writer
            };
        }

        private void CreateServerCommunicationLayer()
        {
            //translator
            var serverTranslator = new NetworkCommandTranslator(SerializationAdapter);
            var serverMessageTranslator = new NetIncomingMessageNetworkCommandConnectionTranslator(serverTranslator);

            //datalayer to gather data out of the game world
            var serverStateDataLayer = new ServerGameStateService<WorldGame>(
                                                AuthenticationService,
                                                AuthenticationService,
                                                this,
                                                SerializationAdapter);
            //hard coded logger here


            GameStateContext = new ServerNetworkGameStateContext<WorldGame>(
                ServerInput,
                ServerOutput,
                serverMessageTranslator,
                serverStateDataLayer,
                GetLogger());

            GameStateContext.Initialize();
        }

        private void BuildSector(string sectorName,
                                string sectorUp,
                                string sectorLeft,
                                string sectorRight,
                                string sectorDown)
        {
            //Factory has to build this
            var anotherSector = sectorName;
            AddSectorStateMachine(anotherSector);
            var anotherSectorStateMachine = GameSectorLayerServices[anotherSector];
            anotherSectorStateMachine.SharedContext = new GameSectorLayerService
            {
                Tag = anotherSector
            };
            anotherSectorStateMachine
                .GetService
                .Get(ServerGameSectorNewBook.BuildDefaultSectorState)
                .Handle(anotherSectorStateMachine);

            var playerShifter = new RouterPlayerShifterMechanic(
                new RouteDualMediator(null, null),
                new List<GameSectorRoutePair>() {
                    new GameSectorRoutePair()
                    {
                        Trespassing = GameSectorTrespassingDirection.Down,
                        GameSectorTag = anotherSector,
                        GameSectorDestinationTag = sectorDown,
                    },
                    new GameSectorRoutePair()
                    {
                        Trespassing = GameSectorTrespassingDirection.Left,
                        GameSectorTag = anotherSector,
                        GameSectorDestinationTag = sectorLeft,
                    },
                    new GameSectorRoutePair()
                    {
                        Trespassing = GameSectorTrespassingDirection.Right,
                        GameSectorTag = anotherSector,
                        GameSectorDestinationTag = sectorRight
                    },
                    new GameSectorRoutePair()
                    {
                        Trespassing = GameSectorTrespassingDirection.Up,
                        GameSectorTag = anotherSector,
                        GameSectorDestinationTag = sectorUp
                    },
                });

            var routeTrespassingMarker = new RouteTrespassingMarkerMechanic(new RouteDualMediator(null, null), 100);
            var mediator = new RouteDualMediator(routeTrespassingMarker, playerShifter);
            routeTrespassingMarker.setRouteMediator(mediator);
            playerShifter.SetRouteMediator(mediator);

            SectorsOwnerMechanics.Add(routeTrespassingMarker);
            SectorsOwnerMechanics.Add(playerShifter);

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


        private void AddSectorStateMachine(string sectorName)
        {
            if (string.IsNullOrWhiteSpace(sectorName))
                throw new ArgumentNullException("sector must have a name");
            if (GameSectorLayerServices.ContainsKey(sectorName))
                throw new SectorAlreadyExistsException(sectorName);

            //Get the sector configuration or overwrites it with the starting sector configuration
            var sectorConfiguration = Configuration.SectorConfigurations.FirstOrDefault(s => s.Tag == sectorName);
            if (sectorConfiguration == null)
                sectorConfiguration = Configuration.SectorConfigurations.FirstOrDefault(s => s.Tag == Configuration.StartingSector);

            GameSectorLayerServices.Add(sectorName, SectorStateMachine.BuildStateMachine(sectorConfiguration));
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

            foreach (var sectorOwnerMechanic in SectorsOwnerMechanics)
                sectorOwnerMechanic.Update(this);

            foreach (var sector in GameSectorLayerServices.Values.Where(sector => sector.SharedContext.CurrentStatus == GameSectorStatus.Running))
            {
                //run the default sector run operation if not specified
                var sectorRunOperation = Configuration.SectorConfigurations.FirstOrDefault(s => s.Tag == sector.SharedContext.Tag)?.RunOperation;
                if(sectorRunOperation == null)
                    sectorRunOperation = Configuration.SectorConfigurations.FirstOrDefault(s => s.Tag == Configuration.StartingSector).RunOperation;

                sector.SharedContext.CurrentGameTime = CurrentGameTime;
                sector
                    .GetService
                    .Get(sectorRunOperation)
                    //  .Get("RunInParallel")
                    .Handle(sector);
            }
            Thread.Sleep(TotalGameTime);
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
                                        return sector
                                                .SharedContext
                                                .IODataLayer
                                                .GetGameStateByLoginToken(loginToken);
                                    }
                                    catch (Exception ex)
                                    {
                                        sector.SharedContext.Messages.Add(ex.Message);
                                        return null;
                                    }
                                });

            foundGameStates = foundGameStates.Where(gameState => gameState != null);

            if (foundGameStates.Any())
            {
                //Console.WriteLine($"{nameof(GetGameStateByLoginToken)}:Found game state ");
                return foundGameStates.First();
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
            GameSectorLayerServices[Configuration.StartingSector]
                    .SharedContext
                    .DataLayer
                    .Players.Add(CreatePlayerSpaceShip(loginToken));
            return GameSectorLayerServices[Configuration.StartingSector]
                    .SharedContext
                    .IODataLayer.RegisterGameStateData(loginToken);
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
                                        .FirstOrDefault(player => player.LoginToken == gameStateData.LoginToken);

                    if (currentPlayer == null)
                        return false;

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

        public IGameSectorLayerService GetSector(string sectorIdentifier) => GameSectorLayerServices[sectorIdentifier].SharedContext;

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
