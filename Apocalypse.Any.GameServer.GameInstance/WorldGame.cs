using Apocalypse.Any.Core;
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
using Microsoft.Extensions.Logging;
using Microsoft.Xna.Framework;
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
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Network;
using CommandPressReleaseTranslator = Apocalypse.Any.Core.Input.CommandPressReleaseTranslator;

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

        private IUserAuthenticationService AuthenticationService { get; set; }

        public GameServerConfiguration ServerConfiguration { get; set; }

        #endregion Server stuff

        #region EntityFactories

        public PlayerSpaceshipFactory PlayerFactory { get; set; } = new PlayerSpaceshipFactory();
        private IByteArraySerializationAdapter SerializationAdapter { get; set; }

        private GameClientConfiguration ClientConfiguration { get; }
        #endregion EntityFactories

        #region SyncClient
        private SyncClient<PlayerSpaceship,EnemySpaceship,Item,Projectile,CharacterEntity,CharacterEntity,ImageData> SendPressReleaseWorker
        {
            get;
            set;
        }

        /// <summary>
        /// Tells if this instance is logged in to a sync server
        /// </summary>
        /// <value></value>
        private bool LoggedInToPressRelease { get; set; }

        /// <summary>
        /// Converts any input to an int command
        /// </summary>
        /// <returns></returns>
        private IntCommandStringCommandTranslator SyncCommandTranslator { get; } = new IntCommandStringCommandTranslator();
        
        private CommandPressReleaseTranslator PressReleaseTranslator { get; } = new CommandPressReleaseTranslator();
        #endregion

        #region Logging / Monitoring

        private ILogger<byte> Logger { get; }
        
        #endregion
        
        public WorldGame(GameServerConfiguration serverConfiguration, GameClientConfiguration clientConfiguration)
        {
            LoggerServiceFactory = new LoggerServiceFactory();
            Logger = LoggerServiceFactory.GetLogger();
            
            ServerConfiguration = serverConfiguration;
            ClientConfiguration = clientConfiguration;

            InitSerializer(serverConfiguration);
            
            InitGameSectorAndSectorStateMachine();
            
            InitAuthenticationService(clientConfiguration);
            
            CreateServer(
                ServerConfiguration.ServerPeerName,
                ServerConfiguration.ServerIp,
                ServerConfiguration.ServerPort);


            InitSectorMechanics();

            //create default starting sector    
            GameSectorRoutePairService = new GameSectorRoutePairService();
            
            BuildSectorGrid();

            RunStartingSectorOnce();

            InitWorldIO();
            
            //Connection to sync server
            InitSyncServerIfNeeded();

            //Language script file
            InitLanguageScript();
            
            CreateGameTimeIfNotExists(null);

            RunAllSectorsOnce();

            //Set starting sector to "run"
            InitStartingSector();

        }

        private void InitStartingSector()
        {
            Logger.LogInformation($"Done loading sectors. Starting sector {ServerConfiguration.StartingSector}");
            GameSectorLayerServices[ServerConfiguration.StartingSector]
                .GetService
                .Get("MarkSectorAsRunning")
                .Handle(GameSectorLayerServices[ServerConfiguration.StartingSector]);
        }

        private void RunAllSectorsOnce()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            foreach (var sector in GameSectorLayerServices.Values)
            {
                sector.SharedContext.CurrentGameTime = CurrentGameTime;
                sector.Run(ServerConfiguration.StartupFunction);
                Console.Write(".");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        private void InitLanguageScript()
        {
            LanguageScriptFileEvaluator languageScriptFileEvaluator = new LanguageScriptFileEvaluator(
                ServerConfiguration.StartupScript, ServerConfiguration.StartupFunction, ServerConfiguration.RunOperation);
            Console.ForegroundColor = ConsoleColor.Yellow;
            foreach (var sector in GameSectorLayerServices.Values)
            {
                languageScriptFileEvaluator.Evaluate(sector);
                Console.Write(".");
            }

            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine("runner starting..." + ServerConfiguration.StartupFunction);
        }

        private void InitSyncServerIfNeeded()
        {
            if (ClientConfiguration != null)
            {
                SendPressReleaseWorker =
                    new SyncClient<PlayerSpaceship, EnemySpaceship, Item, Projectile, CharacterEntity, CharacterEntity,
                        ImageData>(ClientConfiguration, Logger)
                    {
                        LastSectorKey = ServerConfiguration.StartingSector
                    };
                Logger.LogInformation($"Sync client created for {ClientConfiguration.User}");
            }
        }

        private void InitWorldIO()
        {
            var serverStateDataLayer = new ServerGameStateService<WorldGame>(
                AuthenticationService,
                AuthenticationService,
                this,
                SerializationAdapter);

            //translator
            var serverMessageTranslator = GetCommandTranslators();
            GameStateContext = new ServerNetworkGameStateContext<WorldGame>(
                ServerInput,
                ServerOutput,
                serverMessageTranslator,
                serverStateDataLayer,
                Logger);

            GameStateContext.Initialize();
        }

        private void RunStartingSectorOnce()
        {
            var inGameSectorStateMachine = GameSectorLayerServices[ServerConfiguration.StartingSector];
            inGameSectorStateMachine.SharedContext = new GameSectorLayerService
            {
                Tag = ServerConfiguration.StartingSector
            };

            inGameSectorStateMachine
                .GetService
                .Get(ServerConfiguration.BuildOperation)
                .Handle(inGameSectorStateMachine);
        }

        /// <summary>
        /// Initializes the game sector layer service factory and the dictionary containing the game sector layer services (sectors)  
        /// </summary>
        private void InitGameSectorAndSectorStateMachine()
        {
            GameSectorLayerServices = new Dictionary<int, IStateMachine<string, IGameSectorLayerService>>();
            SectorStateMachine = new InMemoryStorageGameSectorLayerServiceFactory();
        }

        /// <summary>
        /// Initializes the authentication service, based on the given configuration. If the game client configuration given is null
        /// If the client configuration is given, the server will be treated as a local server
        /// </summary>
        /// <param name="clientConfiguration"></param>
        private void InitAuthenticationService(GameClientConfiguration clientConfiguration)
        {
            if (clientConfiguration == null)
                AuthenticationService = new ExampleLoginAndRegistrationService();
            else
                AuthenticationService = new ExampleLocalLoginAndRegistrationService();
        }

        /// <summary>
        /// Creates a translator that converts the incoming messages to network command connections / See NetIncomingMessageNetworkCommandConnectionTranslator as an example
        /// </summary>
        /// <returns></returns>
        private NetIncomingMessageNetworkCommandConnectionTranslator GetCommandTranslators()
        {
            var serverTranslator = new NetworkCommandTranslator(SerializationAdapter);
            var serverMessageTranslator = new NetIncomingMessageNetworkCommandConnectionTranslator(serverTranslator);
            return serverMessageTranslator;
        }

        /// <summary>
        /// Builds and loads all the sector mechanics
        /// </summary>
        private void InitSectorMechanics()
        {
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
        }

        /// <summary>
        /// Loads the global serializer used by the game server
        /// </summary>
        /// <param name="serverConfiguration">game server configuration</param>
        /// <exception cref="Exception">Error if the serializer cannot be loaded</exception>
        private void InitSerializer(GameServerConfiguration serverConfiguration)
        {
            var leSerializationType = serverConfiguration.SerializationAdapterType.LoadType(false, false);
            if (leSerializationType == null || leSerializationType.Length == 0)
                throw new Exception("Serializer cannot be loaded");

            var serializerType = leSerializationType.FirstOrDefault() ??
                                 throw new Exception(
                                     $"Cannot load serializer type {serverConfiguration.SerializationAdapterType}");

            SerializationAdapter = Activator.CreateInstance(serializerType) as IByteArraySerializationAdapter;
        }

        /// <summary>
        /// Builds a looping grid out of sectors. By default is 4x4 = 16 Sectors
        /// </summary>
        private void BuildSectorGrid(int size = 4)
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


                    BuildSector(cell);

                    Logger.LogInformation($"x:{x} y:{y} c:{cell} u:{cell + up} l:{cell + left} r:{cell + right} d:{cell + down}");

                    gridTrespassingMechanic.RegisterRoutePair(GameSectorRoutePairService.CreateRoutePair(GameSectorTrespassingDirection.Up, cell, cell + up));
                    gridTrespassingMechanic.RegisterRoutePair(GameSectorRoutePairService.CreateRoutePair(GameSectorTrespassingDirection.Left, cell, cell + left));
                    gridTrespassingMechanic.RegisterRoutePair(GameSectorRoutePairService.CreateRoutePair(GameSectorTrespassingDirection.Right, cell, cell + right));
                    gridTrespassingMechanic.RegisterRoutePair(GameSectorRoutePairService.CreateRoutePair(GameSectorTrespassingDirection.Down, cell, cell + left));

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
            Logger.LogInformation(Server.ToString());
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
        
        //this doesnt belong here. Need refactoring
        private void UpdateSectorOfPlayerInsideSyncClient() {
            if(ClientConfiguration == null) return;
            var sectorKV = GameSectorLayerServices.FirstOrDefault(kv => kv.Value.SharedContext.DataLayer.Players.Any(p => p.LoginToken == SendPressReleaseWorker.LoginToken));
	        if(sectorKV.Value == null) return;
            SendPressReleaseWorker.LastSectorKey = sectorKV.Key;
        }

        public void Update(GameTime gameTime)
        {
            DelegateSyncServerDataToLocalServer();
            CreateGameTimeIfNotExists(gameTime);
            UpdateGameTime(CurrentGameTime);

            TryLoginToSyncServer();

            GameStateContext.Update();
            UpdateSectorOfPlayerInsideSyncClient();
            RunSectorOwnerMechanics();

            var timeToWait = TimeSpan.FromSeconds(ServerConfiguration.ServerUpdateInSeconds);

            RunPopulatedRunningSectorsWithATask(gameTime);

            Thread.Sleep(timeToWait);
        }

        private void RunPopulatedRunningSectorsWithATask(GameTime gameTime)
        {
            foreach (var sector in GameSectorLayerServices
                .Values
                .Where(sector => sector.SharedContext.CurrentStatus == GameSectorStatus.Running &&
                                 sector.SharedContext.DataLayer.Players.Any()))
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
        }

        private void RunSectorOwnerMechanics()
        {
            foreach (var sectorMechanic in SectorsOwnerMechanics)
            {
                sectorMechanic.Update(this);
            }
        }

        private void TryLoginToSyncServer()
        {
            //Try to login to the sync server
            if (!LoggedInToPressRelease && ClientConfiguration != null)
            {
                var loginAttempt = (SendPressReleaseWorker?.TryLogin()).GetValueOrDefault();
                Logger.LogInformation(((int)loginAttempt).ToString());
                LoggedInToPressRelease = loginAttempt == NetSendResult.Sent;
            }
        }

        /// <summary>
        /// Compares a user with the user that plays locally.
        /// If it matches, the commands of the player (player owned by user) will be delegated to the sync server
        /// </summary>
        /// <param name="loginToken">Login token from a player in the sync server</param>
        /// <param name="commands"></param>
        private void DelegatePlayerCommandsToSyncServer(string loginToken, IEnumerable<string> commands)
        {
            if(string.IsNullOrWhiteSpace(loginToken) || commands == null )
                return;
            
            var enumerable = commands as string[] ?? commands.ToArray();
            if(!enumerable.Any())
                return;
            
            if(!LoggedInToPressRelease || ClientConfiguration == null)
                return;

            var user = AuthenticationService.GetByLoginTokenHack(loginToken);

           Logger.LogInformation($"CHECK - {user.Username} against {ClientConfiguration.User.Username} - {user.Password} against {ClientConfiguration.User.Password}");

            //password will never match because password is encrypted and the client configuration password is not!
            if(user.Username != ClientConfiguration.User.Username)
                return;

            var lastSectorOfClient = GameSectorLayerServices.FirstOrDefault(kv =>
                kv.Value.SharedContext.DataLayer.Players.Any(p => p.LoginToken == loginToken)).Key;
            
            //update last sector where the sync clients player was
            if (SendPressReleaseWorker == null)
                return;
                        
            SendPressReleaseWorker.LastSectorKey = lastSectorOfClient;
            SendPressReleaseWorker.ProcessIncomingMessages(enumerable.Select(cmd => SyncCommandTranslator.Translate(cmd)));
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
                    Logger.LogWarning($"Sectors {string.Join(',', problematicGameState.Select(gs => gs.ToString()))} have more than one gamestate");
                }
                return foundGameStates.First().gameState;
            }

            Logger.LogWarning($"{nameof(GetGameStateByLoginToken)}:Found NO game state. Login Token {loginToken}");

            var user = AuthenticationService.GetByLoginTokenHack(loginToken);

            if (user == null)
                throw new NotImplementedException("new users cannot be inserted into this demo");

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
            Logger.LogInformation($"{nameof(RegisterGameStateData)} in World");
            
            var newPlayer = CreatePlayerSpaceShip(loginToken);
            newPlayer.CurrentImage.Position.X = GameSectorLayerServices[ServerConfiguration.StartingSector].SharedContext.SectorBoundaries.MaxSectorX / 2f;
            newPlayer.CurrentImage.Position.Y = GameSectorLayerServices[ServerConfiguration.StartingSector].SharedContext.SectorBoundaries.MaxSectorY / 2f;

            GameSectorLayerServices[ServerConfiguration.StartingSector]
                    .SharedContext
                    .DataLayer
                    .Players.Add(newPlayer);

            FirePlayerRegisteredEvent(newPlayer);
            
            // DelegatePlayerCommandsToSyncServer(loginToken, new List<string>());
                
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

        private GameSectorRoutePairService GameSectorRoutePairService { get; }

        private LoggerServiceFactory LoggerServiceFactory { get; }

        public bool ForwardClientDataToGame(GameStateUpdateData updateData)
        {
            //build it
            var now = DateTime.Now;

            DelegatePlayerCommandsToSyncServer(
                updateData.LoginToken, 
                PressReleaseTranslator.Translate(updateData.Commands));

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


        /// <summary>
        /// Passes all the data layer data from the SyncServer to the sector of the player
        /// </summary>
        private void DelegateSyncServerDataToLocalServer() {
            if(ClientConfiguration == null)
                return;
            if(!SendPressReleaseWorker.NewDataLayer)
                return;

            Logger.LogInformation("PASSING DATA LAYER");

            foreach(var sectorStateMachine in GameSectorLayerServices.Values)
            {
                foreach(var localPlayer in sectorStateMachine.SharedContext.DataLayer.Players)
                {                    
                    foreach(var serverPlayer in SendPressReleaseWorker.DataLayer.Players)
                    {
                        if(serverPlayer.LoginToken != localPlayer.LoginToken)
                            continue;
                        
                        Logger.LogInformation("Passing server player data to local player");
                        //only apply the position and rotation value for testing purpouses
                        localPlayer.CurrentImage.Position.X = serverPlayer.CurrentImage.Position.X;
                        localPlayer.CurrentImage.Position.Y = serverPlayer.CurrentImage.Position.Y;
                        localPlayer.CurrentImage.Rotation.Rotation = serverPlayer.CurrentImage.Rotation.Rotation;
                        localPlayer.CurrentImage.Path = serverPlayer.CurrentImage.Path;
                    }                    
                

                    //we have the players sector. throw everything in the trash and pass the server data
                    if(SendPressReleaseWorker.LoginToken == localPlayer.LoginToken) {
                        foreach(var localEnemy in sectorStateMachine.SharedContext.DataLayer.Enemies)
                        {                    
                            foreach(var serverEnemy  in SendPressReleaseWorker.DataLayer.Enemies)
                            {
                                //Will not match. Every generated enemy will have a local based id
                                if(localEnemy.Id != serverEnemy.Id)
                                    continue;
                                                                                        
                                //only apply the position and rotation value for testing purpouses
                                localEnemy.CurrentImage.Position.X = serverEnemy.CurrentImage.Position.X;
                                localEnemy.CurrentImage.Position.Y = serverEnemy.CurrentImage.Position.Y;
                                localEnemy.CurrentImage.Rotation.Rotation = serverEnemy.CurrentImage.Rotation.Rotation;
                                localEnemy.CurrentImage.Path = serverEnemy.CurrentImage.Path;
                                localEnemy.CurrentImage.Scale = new Vector2(serverEnemy.CurrentImage.Scale.X, serverEnemy.CurrentImage.Scale.Y);
                            }
                        }

                        foreach(var localImageData in sectorStateMachine.SharedContext.DataLayer.ImageData)
                        {                    
                            foreach(var serverImageData  in SendPressReleaseWorker.DataLayer.ImageData)
                            {
                                //Will not match. Every generated enemy will have a local based id
                                if(localImageData.Id != serverImageData.Id)
                                    continue;
                                                                                        
                                //only apply the position and rotation value for testing purpouses
                                localImageData.Position.X = serverImageData.Position.X;
                                localImageData.Position.Y = serverImageData.Position.Y;
                                localImageData.Rotation.Rotation = serverImageData.Rotation.Rotation;
                                localImageData.Path = serverImageData.Path;
                                localImageData.Scale = new Vector2(serverImageData.Scale.X, serverImageData.Scale.Y);
                            }
                        }

                        //and so on...
                    }
		        }
            }
            SendPressReleaseWorker.NewDataLayer = false;
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

                    var playerCount = currentPlayer.Count();
                    if (playerCount == 0)
                        return false;

                    if (playerCount <= 1)
                        return sector
                            .SharedContext
                            .IODataLayer
                            .ForwardServerDataToGame(gameStateData);

                    Logger.LogError("Something is wrong ... player with same login token found in different sectors");

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
            // Logger.LogInformation(message);
        }

        public IGameSectorLayerService GetSector(int sectorIdentifier) => GameSectorLayerServices[sectorIdentifier].SharedContext;
    }
}
