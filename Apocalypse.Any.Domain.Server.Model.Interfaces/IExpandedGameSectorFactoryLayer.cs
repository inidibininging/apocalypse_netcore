using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// Contains all relevant factories bound to a game sector.    
    /// This interface expands the main types with a reflection based generic part.
    /// </summary>
    /// <typeparam name="TPlayer">Type of player</typeparam>
    /// <typeparam name="TEnemy">Type of enemy</typeparam>
    /// <typeparam name="TItem">Type of item</typeparam>
    /// <typeparam name="TProjectile">Type of projectile</typeparam>
    /// <typeparam name="TEntitiesBaseType">Base type of all models</typeparam>
    /// <typeparam name="TGeneralCharacter">Generalized type for an entity</typeparam>
    /// <typeparam name="TImageData">Type, bound to an image (ImageData, no attributes) </typeparam>
    public interface IExpandedGameSectorFactoryLayer<TPlayer,
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
                                             TImageDataKey> :
        IGameSectorFactoryLayer<TPlayer,
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
        , IGenericGameSectorDataLayer
        where TPlayer : TEntitiesBaseType, new()
        where TEnemy : TEntitiesBaseType, new()
        where TItem : TEntitiesBaseType, new()
        where TProjectile : Projectile, new()
        where TGeneralCharacter : TEntitiesBaseType, new()
        where TImageData : ImageData, new()
        where TEntitiesBaseType : CharacterEntity, new()
    {

    }
}
