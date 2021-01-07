using Apocalypse.Any.Client.Services;
using Apocalypse.Any.Client.States.UI.Dialog;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
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
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using States.Core.Common;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apocalypse.Any.Constants;
using Apocalypse.Any.GameServer.States.Sector.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Storage
{
    /// <summary>
    /// Builder of IGameSectorLayerService
    /// </summary>
    public class InMemoryStorageGameSectorLayerServiceFactory : IGameSectorLayerServiceStateMachineFactory<GameServerConfiguration>, IGameSectorLayerServiceFactory
    {
        private const string DialogLocationRelationLayerName = "DialogLocations";
        private const string DropPlayerItemDialogName = "DropPlayerItemDialogEvent";
        private const string PlayerRegisteredEventName = "PlayerRegisteredEvent";
        private const string DefaultSectorBank = "DefaultBank";

        private const string TagNameForDeactivatingMechanics = "NoPlayerInDistance";
        public IStateMachine<string, IGameSectorLayerService> BuildStateMachine(GameServerConfiguration gameServerConfiguration)
        {
            if (gameServerConfiguration == null)
                throw new NotImplementedException();
            return BuildDefaultStateMachine(gameServerConfiguration);
        }

        private IStateMachine<string, IGameSectorLayerService> BuildDefaultStateMachine(GameServerConfiguration gameServerConfiguration)
        {
            var inMemoryStorage = new Dictionary<string, IState<string, IGameSectorLayerService>>();
            var serializer = Activator.CreateInstance(gameServerConfiguration.SerializationAdapterType.LoadType(false, false)[0]) as IByteArraySerializationAdapter;

            inMemoryStorage.Add(nameof(BuildIODataLayerState), new BuildIODataLayerState());
            inMemoryStorage.Add(nameof(BuildFactoriesState), new BuildFactoriesState());
            inMemoryStorage.Add(nameof(BuildSingularMechanicsState), new BuildSingularMechanicsState());
            inMemoryStorage.Add(nameof(BuildMiniCityFactories), new BuildMiniCityFactories(new Client.Services.RectangularFrameGeneratorService(), (Randomness.Instance.From(0,200) > 125 ? ImagePaths.miniCity : ImagePaths.miniCity2)));
            inMemoryStorage.Add(nameof(CreateOrUpdateIdentifiableCircularLocationState), new CreateOrUpdateIdentifiableCircularLocationState(
                                                                                        new RectangularFrameGeneratorService(),
                                                                                        new ImageToRectangleTransformationService(),
                                                                                        DialogLocationRelationLayerName));
            inMemoryStorage.Add(nameof(BuildDataLayerState<GenericGameStateDataLayer>), 
                new BuildDataLayerState<GenericGameStateDataLayer>(
                    DialogLocationRelationLayerName, 
                    DropPlayerItemDialogName,
                    PlayerRegisteredEventName,
                    DefaultSectorBank,
                    () => new IntBankFactory()
                ));
            
            inMemoryStorage.Add(nameof(AddDroppedItemsAsCurrencyToPlayersBankState), new AddDroppedItemsAsCurrencyToPlayersBankState(new ShortCounterThreshold()));
            inMemoryStorage.Add(nameof(CreateOrUpdateItemDialogRelationsState), new CreateOrUpdateItemDialogRelationsState(DropPlayerItemDialogName));
            inMemoryStorage.Add(nameof(CreatePlayerSelectsItemDialogEventState), new CreatePlayerSelectsItemDialogEventState(DropPlayerItemDialogName));

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
            inMemoryStorage.Add("ResetSectorStatus",
                new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        machine.SharedContext.CurrentStatus = GameSectorStatus.StandBy;
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
                    nameof(BuildDataLayerState<GenericGameStateDataLayer>),
                    nameof(BuildIODataLayerState),
                    "BuildPlayerDialogService",
                    "BuildEventDispatcher",
                    "BuildSectorBoundaries",
                    "BuildMaxes",
                    "BuildMessages",
                    nameof(BuildFactoriesState),
                    nameof(BuildMiniCityFactories),
                    nameof(BuildSingularMechanicsState),
                    "ResetSectorStatus"
                }
            });

            inMemoryStorage.Add(nameof(DropItemsState), new DropItemsState());
            inMemoryStorage.Add(nameof(SetMechanicsStatusBasedOnDistanceToPlayerState), new SetMechanicsStatusBasedOnDistanceToPlayerState(TagNameForDeactivatingMechanics, distanceToActivate: 512));
            
            inMemoryStorage.Add(nameof(CreateEnemySpaceShipState), new CreateEnemySpaceShipState(new MockEnemyPreNameGenerator()));
            inMemoryStorage.Add(nameof(CreateRandomPlanetCommand), new CreateRandomPlanetCommand());
            inMemoryStorage.Add(nameof(CreateRandomMediumSpaceShipState), new CreateRandomMediumSpaceShipState());
            inMemoryStorage.Add(nameof(CreateRandomFogCommand), new CreateRandomFogCommand());
            inMemoryStorage.Add(nameof(CreateRandomMiniCityCommand), new CreateRandomMiniCityCommand());

            inMemoryStorage.Add(nameof(UpdateProjectileMechanicsState), new UpdateProjectileMechanicsState());
            
            //Applies all enemy mechanisc to every enemy if the enemy is in players distance
            inMemoryStorage.Add("UpdateEnemyMechanics", new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        foreach (var mech in machine.SharedContext.SingularMechanics.EnemyMechanics)
                        {
                            foreach (var entity in machine.SharedContext.DataLayer.Enemies.Where(e => !e.Tags.Contains(TagNameForDeactivatingMechanics)))
                            {
                                mech.Value.Update(entity);
                            }
                        }
                    })));
            
            // inMemoryStorage.Add("UpdatePlayerMechanics", new CommandStateActionDelegate<string, IGameSectorLayerService>(
            //         new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
            //         {
            //             if (machine.SharedContext.DataLayer.Players.Count == 0)
            //                 return;
            //             
            //             
            //
            //             foreach (var mech in machine.SharedContext.SingularMechanics.PlayerMechanics)
            //             {
            //                 foreach (var entity in machine.SharedContext.DataLayer.Players)
            //                 {
            //                     // var playersView = machine.SharedContext.IODataLayer.GetGameStateByLoginToken(entity.LoginToken);
            //                     
            //                     //Fix for skipping players that are in a dialog
            //                     // if (mech.Key == "thrust_players" && 
            //                     //     entity.Tags.Contains(ProcessPlayerDialogsRequestsState.PlayerOnDialogEvent))                                
            //                     //     continue;
            //                     //
            //                     // mech.Value.Update(entity);
            //                     
            //                 }
            //             }
            //         })));

            inMemoryStorage.Add("UpdateProps", new CommandStateActionDelegate<string, IGameSectorLayerService>(
                    new Action<IStateMachine<string, IGameSectorLayerService>>((machine) =>
                    {
                        foreach (var mech in machine.SharedContext.SingularMechanics.ImageDataMechanics)
                        {
                            foreach (var imageData in machine.SharedContext.DataLayer.ImageData)
                            {
                                if ( (imageData.SelectedFrame.frame == ImagePaths.PlanetFrame ||
                                      imageData.SelectedFrame.frame == ImagePaths.RandomPlanetFrame0 ||
                                      imageData.SelectedFrame.frame == ImagePaths.RandomPlanetFrame1 ||
                                      imageData.SelectedFrame.frame == ImagePaths.RandomPlanetFrame2 ||
                                      imageData.SelectedFrame.frame == ImagePaths.RandomPlanetFrame3
                                      ||
                                     imageData.SelectedFrame.frame == ImagePaths.MiniCityImagePath) &&
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

            inMemoryStorage.Add(nameof(ProcessRotationMapsForPlayerMechanicsState), new ProcessRotationMapsForPlayerMechanicsState());
            inMemoryStorage.Add(nameof(ProcessShootingForPlayerMechanicsState), new ProcessShootingForPlayerMechanicsState());
            inMemoryStorage.Add(nameof(ProcessThrustForPlayerMechanicsState), new ProcessThrustForPlayerMechanicsState());
            inMemoryStorage.Add(nameof(ProcessCollisionMechanicState), new ProcessCollisionMechanicState(new RectangleCollisionMechanic(),
                                                                                            new ImageToRectangleTransformationService()));
            inMemoryStorage.Add(nameof(ProcessPlayerDialogsRequestsState), new ProcessPlayerDialogsRequestsState(DialogLocationRelationLayerName));
            inMemoryStorage.Add(nameof(ProcessPlayerChooseStatState), new ProcessPlayerChooseStatState());
            inMemoryStorage.Add(nameof(ProcessUseInventoryForPlayerState), new ProcessUseInventoryForPlayerState());
            inMemoryStorage.Add(nameof(ProcessInventoryLeftState), new ProcessInventoryLeftState());
            inMemoryStorage.Add(nameof(ProcessInventoryRightState), new ProcessInventoryRightState());
            inMemoryStorage.Add(nameof(ProcessReleaseStatState), new ProcessReleaseStatState());
            inMemoryStorage.Add(nameof(ProcessDeadPlayer), new ProcessDeadPlayer());

            inMemoryStorage.Add(nameof(RemoveImagesMechanicsState), new RemoveImagesMechanicsState());
            inMemoryStorage.Add(nameof(RemoveDeadEnemiesMechanicsState), new RemoveDeadEnemiesMechanicsState());
            inMemoryStorage.Add(nameof(SectorCsvLoggerState),new SectorCsvLoggerState());
            
            inMemoryStorage.Add("ProcessPlayerInputBeforeThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                    //process player input
                    nameof(ProcessRotationMapsForPlayerMechanicsState),
                    nameof(ProcessShootingForPlayerMechanicsState),
                    nameof(ProcessThrustForPlayerMechanicsState),
                }
            });

            inMemoryStorage.Add("ProcessPlayerInputAfterThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                    //send information to the client
                    nameof(UpdateGameStateDataState),

                    //process other player input
                    nameof(ProcessUseInventoryForPlayerState),
                    nameof(ProcessCollisionMechanicState),
                    nameof(ProcessPlayerChooseStatState),

                    //dialog related states
                    nameof(CreateOrUpdateIdentifiableCircularLocationState),
                    nameof(CreateOrUpdateItemDialogRelationsState),
                    nameof(CreatePlayerSelectsItemDialogEventState),
                    nameof(ProcessPlayerDialogsRequestsState),
                    nameof(AddDroppedItemsAsCurrencyToPlayersBankState),

                    nameof(ProcessInventoryLeftState),
                    nameof(ProcessInventoryRightState),
                    nameof(ProcessReleaseStatState),
                    nameof(ProcessDeadPlayer),
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
                    // "UpdatePlayerMechanics",
                }
            });

            inMemoryStorage.Add("ProjectilesThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                    //projectiles
                    nameof(UpdateProjectileMechanicsState),
                    nameof(RemoveDestroyedProjectilesState),
                }
            });

            inMemoryStorage.Add("GarbageThread", new RoutineState<string, IGameSectorLayerService>()
            {
                Operations = new List<string>()
                {
                     //remove junk
                     nameof(DropItemsState),
                     nameof(RemoveImagesMechanicsState),
                     nameof(RemoveDeadEnemiesMechanicsState),
                     "ConsumeItemExperienceState"
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

                        var workStack = tasks.Select(taskName => Task.Factory.StartNew(() => machine.GetService.Get(taskName).Handle(machine))).ToList();
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
                    nameof(SetMechanicsStatusBasedOnDistanceToPlayerState),
                    // "UpdatePlayerMechanics",

                    //process player input
                    nameof(ProcessRotationMapsForPlayerMechanicsState),
                    nameof(ProcessShootingForPlayerMechanicsState),
                    nameof(ProcessThrustForPlayerMechanicsState),

                    //projectiles
                    nameof(UpdateProjectileMechanicsState),
                    nameof(RemoveDestroyedProjectilesState),

                    //send information to the client
                    nameof(UpdateGameStateDataState),

                    //process other player input
                    nameof(ProcessUseInventoryForPlayerState),
                    nameof(ProcessCollisionMechanicState),
                    nameof(ProcessPlayerChooseStatState),

                    //dialog related states
                    nameof(CreateOrUpdateIdentifiableCircularLocationState),
                    nameof(CreateOrUpdateItemDialogRelationsState),
                    nameof(CreatePlayerSelectsItemDialogEventState),
                    nameof(ProcessPlayerDialogsRequestsState),
                    nameof(AddDroppedItemsAsCurrencyToPlayersBankState),

                    nameof(ProcessInventoryLeftState),
                    nameof(ProcessInventoryRightState),
                    nameof(ProcessReleaseStatState),
                    nameof(ProcessDeadPlayer),

                    //remove junk
                    nameof(DropItemsState),
                    nameof(RemoveImagesMechanicsState),
                    nameof(RemoveDeadEnemiesMechanicsState),
                    "ConsumeItemExperienceState",
                    nameof(SectorCsvLoggerState)
                 }
            });

            var getDelegation = new GetGameSectorNewDelegate(() => inMemoryStorage);
            var setDelegation = new SetGameSectorNewDelegate(() => inMemoryStorage);
            var newDelegation = new NewGameSectorNewDelegate(() => inMemoryStorage);
            
            return new GameSectorContext(getDelegation, setDelegation, newDelegation);
        }
    }
}
