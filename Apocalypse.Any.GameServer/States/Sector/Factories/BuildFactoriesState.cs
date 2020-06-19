using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Model.RPG;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Proxy;
using States.Core.Infrastructure.Services;
using System.Collections.Generic;

namespace Apocalypse.Any.GameServer.States.Sector.Factories
{
    public class BuildFactoriesState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            if (machine.SharedContext == null)
            {
                machine.SharedContext.Messages.Add("game sector is not available");
                return;
            }

            machine.SharedContext.Factories = new InMemoryGameSectorFactoryLayer();

            machine.SharedContext.Factories.PlayerFactory = new Dictionary<string, IGenericTypeFactory<PlayerSpaceship>>();
            machine.SharedContext.Factories.EnemyFactory = new Dictionary<string, IGenericTypeFactory<EnemySpaceship>>();
            machine.SharedContext.Factories.ProjectileFactory = new Dictionary<string, IGenericTypeFactory<Projectile>>();
            machine.SharedContext.Factories.ItemFactory = new Dictionary<string, IGenericTypeFactory<Item>>();
            machine.SharedContext.Factories.ImageDataFactory = new Dictionary<string, IGenericTypeFactory<ImageData>>();
            machine.SharedContext.Factories.GeneralCharacterFactory = new Dictionary<string, IGenericTypeFactory<CharacterEntity>>();

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
            machine.SharedContext.Factories.EnemyFactory.Add(nameof(RandomEnemySpaceshipFactory), new RandomEnemySpaceshipFactory());
            machine.SharedContext.Factories.EnemyFactory.Add(nameof(UncofiguredEnemyFactory), new UncofiguredEnemyFactory());
            machine.SharedContext.Factories.ImageDataFactory.Add(nameof(RandomPlanetFactory), new RandomPlanetFactory());
            machine.SharedContext.Factories.ImageDataFactory.Add(nameof(RandomMediumSpaceshipFactory), new RandomMediumSpaceshipFactory());
            machine.SharedContext.Factories.ImageDataFactory.Add(nameof(RandomFogFactory),new RandomFogFactory());
            machine.SharedContext.Factories.ProjectileFactory.Add(nameof(ProjectileFactory), new ProjectileFactory(new ThrustMechanic()));


        }
    }
}