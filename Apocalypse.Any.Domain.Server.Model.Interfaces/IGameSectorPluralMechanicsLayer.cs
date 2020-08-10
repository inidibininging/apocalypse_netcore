using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IGameSectorPluralMechanicsLayer<TMechanicIdentifier, TGameSectorDataLayer, TPlayer, TEnemy, TItem, TProjectile, TEntitiesBaseType, TProp, TImageData>
        where TGameSectorDataLayer : IGameSectorDataLayer<TPlayer, TEnemy, TItem, TProjectile, TEntitiesBaseType, TProp, TImageData>
        where TPlayer : TEntitiesBaseType, new()
        where TEnemy : TEntitiesBaseType, new()
        where TItem : TEntitiesBaseType, new()
        where TProjectile : Projectile, new()
        where TProp : TEntitiesBaseType, new()
        where TImageData : ImageData, new()
        where TEntitiesBaseType : CharacterEntity, new()
    {
        Dictionary<TMechanicIdentifier, IEnumerableCharacterEntityMechanic<TEntitiesBaseType>> PropsMechanics { get; set; }
        Dictionary<TMechanicIdentifier, IEnumerableCharacterEntityMechanic<TPlayer>> PlayerMechanics { get; set; }
        Dictionary<TMechanicIdentifier, IEnumerableCharacterEntityMechanic<TEnemy>> EnemyMechanics { get; set; }
        Dictionary<TMechanicIdentifier, IEnumerableCharacterEntityMechanic<TItem>> ItemMechanics { get; set; }
        Dictionary<TMechanicIdentifier, IEnumerableMechanic<TProjectile, Projectile>> ProjectileMechanics { get; set; }
        Dictionary<TMechanicIdentifier, IEnumerableFullPositionHolderMechanic<TImageData>> UnnamedPropMechanics { get; set; }

    }
}