using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy;
using States.Core.Infrastructure.Services;
using System.Collections.Generic;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.ItemMechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Transformations;

namespace Apocalypse.Any.GameServer.States.Sector.Factories
{
    /// <summary>
    /// Creates all factories needed in the game
    /// </summary>
    public class BuildFactoriesState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (machine.SharedContext == null)
            {
                machine.SharedContext?.Messages.Add("game sector is not available");
                return;
            }

            machine.SharedContext.Factories = new InMemoryGameSectorFactoryLayer
            {
                PlayerFactory = new Dictionary<string, IGenericTypeFactory<PlayerSpaceship>>(),
                EnemyFactory = new Dictionary<string, IGenericTypeFactory<EnemySpaceship>>(),
                ProjectileFactory = new Dictionary<string, IGenericTypeFactory<Projectile>>(),
                ItemFactory = new Dictionary<string, IGenericTypeFactory<Item>>(),
                ImageDataFactory = new Dictionary<string, IGenericTypeFactory<ImageData>>(),
                GeneralCharacterFactory = new Dictionary<string, IGenericTypeFactory<CharacterEntity>>()
            };


            machine.SharedContext.Factories.PlayerFactory.Add(nameof(PlayerSpaceshipFactory), new PlayerSpaceshipFactory());

            var dropMechanics = new DropProxyMechanic<CharacterEntity>(new DropItemIfHealthIsBelowMinMechanic(
                                                                            new MockItemFactory(
                                                                                new CharacterSheetFactory(),
                                                                                new SectorRandomPositionFactory(),
                                                                                new MockItemPostNameDecoratorGenerator()),
                                                                            new RectangleToSectorBoundaryTransformationService(),
                                                                            new BoundingBoxTransformationService())){
                                                                                Offset = 256
                                                                            };

            machine.SharedContext.Factories.ItemFactory.Add(nameof(DropProxyMechanic<CharacterEntity>), dropMechanics);
            machine.SharedContext.Factories.ItemFactory.Add(nameof(MockItemFactory), new MockItemFactory(
                                                                                new CharacterSheetFactory(),
                                                                                new SectorRandomPositionFactory(),
                                                                                new MockItemPostNameDecoratorGenerator()));
            machine.SharedContext.Factories.EnemyFactory.Add(nameof(RandomEnemySpaceshipFactory), new RandomEnemySpaceshipFactory());
            machine.SharedContext.Factories.EnemyFactory.Add(nameof(UnconfiguredEnemyFactory), new UnconfiguredEnemyFactory());
            machine.SharedContext.Factories.ImageDataFactory.Add(nameof(RandomPlanetFactory), new RandomPlanetFactory());
            machine.SharedContext.Factories.ImageDataFactory.Add(nameof(RandomMediumSpaceshipFactory), new RandomMediumSpaceshipFactory());
            machine.SharedContext.Factories.ImageDataFactory.Add(nameof(RandomFogFactory),new RandomFogFactory());            
            machine.SharedContext.Factories.ProjectileFactory.Add(nameof(ProjectileFactory), new ProjectileFactory(new ThrustMechanic()));


        }
    }
}