using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System.Collections.Generic;

namespace Apocalypse.Any.GameServer.States.Sector.Factories
{
    public class InMemoryGameSectorFactoryLayer :
        IGameSectorFactoryLayer<PlayerSpaceship,
                                EnemySpaceship,
                                Item,
                                Projectile,
                                CharacterEntity,
                                CharacterEntity,
                                ImageData,
                                string,
                                string,
                                string,
                                string,
                                string,
                                string>
    {
        public IDictionary<string, IGenericTypeFactory<PlayerSpaceship>> PlayerFactory { get; set; }
        public IDictionary<string, IGenericTypeFactory<EnemySpaceship>> EnemyFactory { get; set; }
        public IDictionary<string, IGenericTypeFactory<Item>> ItemFactory { get; set; }
        public IDictionary<string, IGenericTypeFactory<Projectile>> ProjectileFactory { get; set; }
        public IDictionary<string, IGenericTypeFactory<CharacterEntity>> GeneralCharacterFactory { get; set; }
        public IDictionary<string, IGenericTypeFactory<ImageData>> ImageDataFactory { get; set; }
    }
}