using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IGameSectorFactoryLayer<TPlayer,
                                             TEnemy,
                                             TItem,
                                             TProjectile,
                                             TEntitiesBaseType,
                                             TGeneralCharacter,
                                             TImageData,

                                             TPlayerKey,
                                             TEnemyKey,
                                             TItemKey,
                                             TProjectileKey,
                                             TGeneralCharacterKey,
                                             TImageDataKey>

        //TPlayerParam,
        //TEnemyParam,
        //TItemParam,
        //TProjectileParam,
        //TGeneralCharacterParam,
        //TImageDataParam>
        where TPlayer : TEntitiesBaseType, new()
        where TEnemy : TEntitiesBaseType, new()
        where TItem : TEntitiesBaseType, new()
        where TProjectile : Projectile, new()
        where TGeneralCharacter : TEntitiesBaseType, new()
        where TImageData : ImageData, new()
        where TEntitiesBaseType : CharacterEntity, new()
    {
        IDictionary<TPlayerKey, IGenericTypeFactory<TPlayer>> PlayerFactory { get; set; }
        IDictionary<TEnemyKey, IGenericTypeFactory<TEnemy>> EnemyFactory { get; set; }
        IDictionary<TItemKey, IGenericTypeFactory<TItem>> ItemFactory { get; set; }
        IDictionary<TProjectileKey, IGenericTypeFactory<TProjectile>> ProjectileFactory { get; set; }
        IDictionary<TGeneralCharacterKey, IGenericTypeFactory<TGeneralCharacter>> GeneralCharacterFactory { get; set; }
        IDictionary<TImageDataKey, IGenericTypeFactory<TImageData>> ImageDataFactory { get; set; }
    }
}