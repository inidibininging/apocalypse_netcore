using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// Provides a interface to all factories in a game sector
    /// </summary>
    /// <typeparam name="TPlayer"></typeparam>
    /// <typeparam name="TEnemy"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    /// <typeparam name="TProjectile"></typeparam>
    /// <typeparam name="TEntitiesBaseType"></typeparam>
    /// <typeparam name="TGeneralCharacter"></typeparam>
    /// <typeparam name="TImageData"></typeparam>
    /// <typeparam name="TPlayerKey"></typeparam>
    /// <typeparam name="TEnemyKey"></typeparam>
    /// <typeparam name="TItemKey"></typeparam>
    /// <typeparam name="TProjectileKey"></typeparam>
    /// <typeparam name="TGeneralCharacterKey"></typeparam>
    /// <typeparam name="TImageDataKey"></typeparam>
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