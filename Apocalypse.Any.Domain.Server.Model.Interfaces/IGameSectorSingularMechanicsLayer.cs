using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using System.Collections.Generic;
using System.ComponentModel;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// Contains all singular mechanics that involve different types of entities in a game
    /// </summary>
    /// <typeparam name="TMechanicIdentifier">Identifier type for searching for mechanics</typeparam>
    /// <typeparam name="TGameSectorDataLayer"></typeparam>
    /// <typeparam name="TPlayer">Player type model</typeparam>
    /// <typeparam name="TEnemy">Enemy type model</typeparam>
    /// <typeparam name="TItem">Item type model</typeparam>
    /// <typeparam name="TProjectile">Projectile type model</typeparam>
    /// <typeparam name="TEntitiesBaseType">Base type for entities</typeparam>
    /// <typeparam name="TProp">Type used for prop models</typeparam>
    /// <typeparam name="TImageData">Type used for image data</typeparam>
    public interface IGameSectorSingularMechanicsLayer<TMechanicIdentifier, TGameSectorDataLayer, TPlayer, TEnemy, TItem, TProjectile, TEntitiesBaseType, TProp, TImageData>
        where TGameSectorDataLayer : IGameSectorDataLayer<TPlayer, TEnemy, TItem, TProjectile, TEntitiesBaseType, TProp, TImageData>
        where TPlayer : TEntitiesBaseType, new()
        where TEnemy : TEntitiesBaseType, new()
        where TItem : TEntitiesBaseType, new()
        where TProjectile : Projectile, new()
        where TProp : TEntitiesBaseType, new()
        where TImageData : ImageData, new()
        where TEntitiesBaseType : CharacterEntity, new()
    {
        Dictionary<TMechanicIdentifier, ISingleCharacterEntityMechanic<TEntitiesBaseType>> PropsMechanics { get; set; }
        Dictionary<TMechanicIdentifier, ISingleCharacterEntityMechanic<TPlayer>> PlayerMechanics { get; set; }
        Dictionary<TMechanicIdentifier, ISingleCharacterEntityMechanic<TEnemy>> EnemyMechanics { get; set; }
        Dictionary<TMechanicIdentifier, ISingleCharacterEntityMechanic<TItem>> ItemMechanics { get; set; }
        Dictionary<TMechanicIdentifier, ISingleEntityWithImageMechanic<TProjectile>> ProjectileMechanics { get; set; }
        Dictionary<TMechanicIdentifier, ISingleFullPositionHolderMechanic<TImageData>> ImageDataMechanics { get; set; }
        
        
    }
}