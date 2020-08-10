using Apocalypse.Any.Client.Services;
using Apocalypse.Any.Client.States.UI.Dialog;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Domain.Server.DataLayer;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.GameServer.States.Sector.Factories;
using Apocalypse.Any.GameServer.States.Sector.Mechanics;
using Apocalypse.Any.GameServer.States.Sector.Mechanics.EnemyMechanics;
using Apocalypse.Any.GameServer.States.Sector.Mechanics.ItemMechanics;
using Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics;
using Apocalypse.Any.GameServer.States.Sector.Mechanics.ProjectileMechanics;
using Apocalypse.Any.GameServer.States.Services;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.PubSub;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.ProjectileMechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Transformations;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using States.Core.Common;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.GameServer.States.Sector.Storage
{
    /// <summary>
    /// Builder for IGameSectorLayerService
    /// </summary>
    public class InMemoryStorageGameSectorLayerServiceFactory : IGameSectorLayerServiceStateMachineFactory<GameServerConfiguration>, IGameSectorLayerServiceFactory
    {
        private const string DialogLocationRelationLayerName = "DialogLocations";

        public IStateMachine<string, IGameSectorLayerService> BuildStateMachine(GameServerConfiguration gameServerConfiguration)
        {
            if (gameServerConfiguration == null)
                throw new NotImplementedException();
            return BuildDefaultStateMachine(gameServerConfiguration);
        }

        private IStateMachine<string, IGameSectorLayerService> BuildDefaultStateMachine(GameServerConfiguration gameServerConfiguration)
        {
            var inMemoryStorage = new Dictionary<string, IState<string, IGameSectorLayerService>>();
            var serializer = Activator.CreateInstance(gameServerConfiguration.SerializationAdapterType.LoadType(true, false)[0]) as ISerializationAdapter;

            inMemoryStorage.Add(ServerGameSectorNewBook.BuildGameStateDataLayerState, new BuildGameStateDataLayerState());
            

            inMemoryStorage.Add(ServerGameSectorNewBook.BuildFactoriesState, new BuildFactoriesState());
            inMemoryStorage.Add(ServerGameSectorNewBook.BuildSingularMechanicsState, new BuildSingularMechanicsState());
            inMemoryStorage.Add(nameof(BuildMiniCityFactories), new BuildMiniCityFactories(new Client.Services.RectangularFrameGeneratorService(), $"Image/miniCity{ (Randomness.Instance.From(0,200) > 125 ? "2" : "") }"));
            inMemoryStorage.Add(nameof(CreateOrUpdateIdentifiableCircularLocationState), new CreateOrUpdateIdentifiableCircularLocationState(
                                                                                        new RectangularFrameGeneratorService(),
                                                                                        new ImageToRectangleTransformationService(),
                                                                                        DialogLocationRelationLayerName));
            inMemoryStorage.Add(ServerGameSectorNewBook.BuildDataLayerState, new BuildDataLayerState<GenericGameStateDataLayer>(
                    new List<string>() { "DropPlayerItemDialogEvent", "PlayerRegisteredEvent" },
                    DialogLocationRelationLayerName
                ));
            inMemoryStorage.Add(nameof(CreateOrUpdateItemDialogRelationsState), new CreateOrUpdateItemDialogRelationsState("DropPlayerItemDialogEvent"));
            inMemoryStorage.Add(nameof(CreatePlayerSelectsItemDialogEventState), new CreatePlayerSelectsItemDialogEventState("DropPlayerItemDialogEvent"));

            inMemoryStorage.Add("BuildPlayerDialogService",
                new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        machine.SharedContext.PlayerDialogService = new ExamplePlayerDialogService(() => (machine.SharedContext.DataLayer.Layers.FirstOrDefault(l => (l as IDialogService) != null) as IDialogService));                        
                    })));
            inMemoryStorage.Add("BuildEventDispatcher",
                new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        machine.SharedContext.EventDispatcher = new EventDispatcher(
                             () => machine
                                    .SharedContext
                                    .DataLayer
                                    .Layers
                                    .OfType<EventQueue>(),
                             () => machine
                                    .SharedContext
                                    .DataLayer
                                    .Layers
                                    .SelectMany(l => l.DataAsEnumerable<INotifiable<string, EventQueueArgument>>())
                                );
                    })));

            inMemoryStorage.Add("BuildSectorBoundaries",
                new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        machine.SharedContext.SectorBoundaries = new SectorBoundary()
                        {
                            MinSectorX = 0,
                            MaxSectorX = gameServerConfiguration.SectorXSize,
                            MinSectorY = 0,
                            MaxSectorY = gameServerConfiguration.SectorYSize
                        };
                    })));
            inMemoryStorage.Add("BuildMaxes",
                new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        machine.SharedContext.MaxEnemies = 64;
                        machine.SharedContext.MaxPlayers = 4;
                    })));
            inMemoryStorage.Add("BuildMessages",
                new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        machine.SharedContext.Messages = new List<string>();
                    })));
            inMemoryStorage.Add("MarkSectorAsRunning",
                new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        machine.SharedContext.CurrentStatus = GameSectorStatus.Running;
                    })));
            inMemoryStorage.Add("BuildLogger",
                new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {

                    })));

            inMemoryStorage.Add(ServerGameSectorNewBook.BuildDefaultSectorState, new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                    ServerGameSectorNewBook.BuildDataLayerState,
                    ServerGameSectorNewBook.BuildGameStateDataLayerState,
                    "BuildPlayerDialogService",
                    "BuildEventDispatcher",
                    "BuildSectorBoundaries",
                    "BuildMaxes",
                    "BuildMessages",
                    ServerGameSectorNewBook.BuildFactoriesState,
                    nameof(BuildMiniCityFactories),
                    ServerGameSectorNewBook.BuildSingularMechanicsState,
                    "MarkSectorAsRunning"
                }
            });

            inMemoryStorage.Add(ServerGameSectorNewBook.DropItemsState, new DropItemsState());

            //inMemoryStorage.Add(ServerGameSectorNewBook.CreatePlayerSpaceshipState, new CreatePlayerSpaceshipCommand());
            inMemoryStorage.Add(ServerGameSectorNewBook.CreateEnemySpaceshipState, new CreateEnemyCommand(new MockEnemyPreNameGenerator()));
            inMemoryStorage.Add(ServerGameSectorNewBook.CreateRandomPlanetState, new CreateRandomPlanetCommand());
            inMemoryStorage.Add(ServerGameSectorNewBook.CreateRandomMediumSpaceShipState, new CreateRandomMediumSpaceShipCommand());
            inMemoryStorage.Add(ServerGameSectorNewBook.CreateRandomFogCommand, new CreateRandomFogCommand());
            inMemoryStorage.Add(nameof(CreateRandomMiniCityCommand), new CreateRandomMiniCityCommand());

            //update ALL!!!!
            // inMemoryStorage.Add(ServerGameSectorNewBook.UpdateAllSingularEnemyMechanicsState, new UpdateAllSingularEnemyMechanicsState());

            inMemoryStorage.Add(ServerGameSectorNewBook.UpdateProjectileMechanicsState, new UpdateProjectileMechanicsState());
            inMemoryStorage.Add("UpdateEnemyMechanics", new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        foreach (var mech in machine.SharedContext.SingularMechanics.EnemyMechanics)
                        {
                            foreach (var entity in machine.SharedContext.DataLayer.Enemies)
                            {
                                mech.Value.Update(entity);
                            }
                        }
                    })));
            inMemoryStorage.Add("UpdatePlayerMechanics", new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        foreach (var mech in machine.SharedContext.SingularMechanics.PlayerMechanics)
                        {
                            foreach (var entity in machine.SharedContext.DataLayer.Players)
                            {
                                //Fix for skipping players that are in a dialog
                                if (mech.Key == "thrust_players" && entity.Tags.Contains(ProcessPlayerDialogsRequestsState.PlayerOnDialogEvent))
                                {
                                    continue;
                                }
                                mech.Value.Update(entity);
                            }
                        }
                    })));

            //inMemoryStorage.Add("UpdateProps", 
            //    new CommandStateActionDelegate<string, IGameSectorLayerService>(
            //        new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>{ })));

            inMemoryStorage.Add("UpdateProps", new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        foreach (var mech in machine.SharedContext.SingularMechanics.ImageDataMechanics)
                        {
                            foreach (var imageData in machine.SharedContext.DataLayer.ImageData)
                            {
                                if ( (imageData.SelectedFrame.Contains("planet", StringComparison.OrdinalIgnoreCase) ||
                                     imageData.SelectedFrame.Contains("miniCity", StringComparison.OrdinalIgnoreCase)) &&
                                     mech.Key == "move_props_around")
                                {
                                    continue;
                                }
                                mech.Value.Update(imageData);
                            }
                        }
                    })));

            inMemoryStorage.Add(ServerGameSectorNewBook.UpdateGameStateDataState, new UpdateGameStateDataState(new PlayerSpaceshipUpdateGameStateFactory(serializer, new ImageToRectangleTransformationService())));
            inMemoryStorage.Add(ServerGameSectorNewBook.RemoveDestroyedProjectilesState, new RemoveDestroyedProjectilesState(new DestroyedProjectilesIterator()));

            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessRotationMapsForPlayerMechanicsState, new ProcessRotationMapsForPlayerMechanicsState());
            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessShootingForPlayerMechanicsState, new ProcessShootingForPlayerMechanicsState());
            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessThrustForPlayerMechanicsState, new ProcessThrustForPlayerMechanicsState());
            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessCollisionMechanicState, new ProcessCollisionMechanicState(new RectangleCollisionMechanic(),
                                                                                            new ImageToRectangleTransformationService()));
            inMemoryStorage.Add(nameof(ProcessPlayerDialogsRequestsState), new ProcessPlayerDialogsRequestsState(DialogLocationRelationLayerName));
            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessPlayerChooseStatState, new ProcessPlayerChooseStatState());
            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessUseInventoryForPlayerState, new ProcessUseInventoryForPlayerState());
            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessInventoryLeftState, new ProcessInventoryLeftState());
            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessInventoryRightState, new ProcessInventoryRightState());
            inMemoryStorage.Add(ServerGameSectorNewBook.ProcessReleaseStatState, new ProcessReleaseStatState());
            inMemoryStorage.Add(nameof(ProcessDeadPlayer), new ProcessDeadPlayer());

            inMemoryStorage.Add(ServerGameSectorNewBook.RemoveImagesMechanicsState, new RemoveImagesMechanicsState());
            inMemoryStorage.Add(ServerGameSectorNewBook.RemoveDeadEnemiesMechanicsState, new RemoveDeadEnemiesMechanicsState());

            inMemoryStorage.Add("ProcessPlayerInputBeforeThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                    //process player input
                    ServerGameSectorNewBook.ProcessRotationMapsForPlayerMechanicsState,
                    ServerGameSectorNewBook.ProcessShootingForPlayerMechanicsState,
                    ServerGameSectorNewBook.ProcessThrustForPlayerMechanicsState,
                }
            });

            inMemoryStorage.Add("ProcessPlayerInputAfterThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                    //send information to the client
                    ServerGameSectorNewBook.UpdateGameStateDataState,

                    //process other player input
                    ServerGameSectorNewBook.ProcessUseInventoryForPlayerState,
                    ServerGameSectorNewBook.ProcessCollisionMechanicState,
                    ServerGameSectorNewBook.ProcessPlayerChooseStatState,
                    ServerGameSectorNewBook.ProcessInventoryLeftState,
                    ServerGameSectorNewBook.ProcessInventoryRightState,
                    ServerGameSectorNewBook.ProcessReleaseStatState,
                }
            });

            inMemoryStorage.Add("ConsumeItemExperienceState", new ConsumeItemExperienceState());

            inMemoryStorage.Add("ServerInternalStuffThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                    //server internal stuff
                    "UpdateEnemyMechanics",
                    "UpdateProps",
                    "UpdatePlayerMechanics",
                }
            });

            inMemoryStorage.Add("ProjectilesThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                    //projectiles
                    ServerGameSectorNewBook.UpdateProjectileMechanicsState,
                    ServerGameSectorNewBook.RemoveDestroyedProjectilesState,
                }
            });

            inMemoryStorage.Add("GarbageThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                     //remove junk
                    ServerGameSectorNewBook.DropItemsState,
                    ServerGameSectorNewBook.RemoveImagesMechanicsState,
                    ServerGameSectorNewBook.RemoveDeadEnemiesMechanicsState
                }
            });

            inMemoryStorage.Add("RunInParallel", new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {

                        var tasks = new List<string>() {
                                    "ProcessPlayerInputAfterThread",
                                    "ServerInternalStuffThread",
                                    "ProcessPlayerInputBeforeThread",
                                    "ProjectilesThread",
                                    "ProcessPlayerInputAfterThread",
                                    "GarbageThread",
                                };

                        var workStack = tasks.Select(taskName => new Task(() => machine.GetService.Get(taskName).Handle(machine))).ToList();
                        workStack.ForEach(task => task.Start());
                        while (workStack.Any(work => !work.IsCompleted))
                        {
                            // Console.WriteLine("Working in parallel....");
                        }
                    })));

            //main loop
            inMemoryStorage.Add(ServerGameSectorNewBook.RunAsDefaultSector, new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                 {
                    //server internal stuff
                    "UpdateEnemyMechanics",
                    "UpdateProps",
                    "UpdatePlayerMechanics",
                    //ServerGameSectorNewBook.UpdateAllSingularEnemyMechanicsState,

                    //process player input
                    ServerGameSectorNewBook.ProcessRotationMapsForPlayerMechanicsState,
                    ServerGameSectorNewBook.ProcessShootingForPlayerMechanicsState,
                    ServerGameSectorNewBook.ProcessThrustForPlayerMechanicsState,

                     //projectiles
                    ServerGameSectorNewBook.UpdateProjectileMechanicsState,
                    ServerGameSectorNewBook.RemoveDestroyedProjectilesState,

                    //send information to the client
                    ServerGameSectorNewBook.UpdateGameStateDataState,

                    //process other player input
                    ServerGameSectorNewBook.ProcessUseInventoryForPlayerState,
                    ServerGameSectorNewBook.ProcessCollisionMechanicState,
                    ServerGameSectorNewBook.ProcessPlayerChooseStatState,
                    nameof(CreateOrUpdateIdentifiableCircularLocationState),
                    nameof(CreateOrUpdateItemDialogRelationsState),
                    nameof(CreatePlayerSelectsItemDialogEventState),
                    nameof(ProcessPlayerDialogsRequestsState),
                    ServerGameSectorNewBook.ProcessInventoryLeftState,
                    ServerGameSectorNewBook.ProcessInventoryRightState,
                    ServerGameSectorNewBook.ProcessReleaseStatState,
                    nameof(ProcessDeadPlayer),

                    //remove junk
                    ServerGameSectorNewBook.DropItemsState,
                    ServerGameSectorNewBook.RemoveImagesMechanicsState,
                    ServerGameSectorNewBook.RemoveDeadEnemiesMechanicsState,
                    "ConsumeItemExperienceState"                    
                 }
            });

            var getDelegation = new GetGameSectorNewDelegate(() => inMemoryStorage);
            var setDelegation = new SetGameSectorNewDelegate(() => inMemoryStorage);
            var newDelegation = new NewGameSectorNewDelegate(() => inMemoryStorage);

            return new GameSectorContext(getDelegation, setDelegation, newDelegation);
        }
    }
}