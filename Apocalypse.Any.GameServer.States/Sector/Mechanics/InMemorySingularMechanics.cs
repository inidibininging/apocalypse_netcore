using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System.Collections.Generic;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class InMemorySingularMechanics :
        IGameSectorSingularMechanicsLayer<string,
                                          IExpandedGameSectorDataLayer<
                                              PlayerSpaceship,
                                              EnemySpaceship,
                                              Item,
                                              Projectile,
                                              CharacterEntity,
                                              CharacterEntity,
                                              ImageData>,
                                          PlayerSpaceship,
                                          EnemySpaceship,
                                          Item,
                                          Projectile,
                                          CharacterEntity,
                                          CharacterEntity,
                                          ImageData>
    {
        public Dictionary<string, ISingleCharacterEntityMechanic<CharacterEntity>> PropsMechanics { get; set; }
        public Dictionary<string, ISingleCharacterEntityMechanic<PlayerSpaceship>> PlayerMechanics { get; set; }
        public Dictionary<string, ISingleCharacterEntityMechanic<EnemySpaceship>> EnemyMechanics { get; set; }
        public Dictionary<string, ISingleCharacterEntityMechanic<Item>> ItemMechanics { get; set; }
        public Dictionary<string, ISingleEntityWithImageMechanic<Projectile>> ProjectileMechanics { get; set; }
        public Dictionary<string, ISingleFullPositionHolderMechanic<ImageData>> ImageDataMechanics { get; set; }        
    }
}