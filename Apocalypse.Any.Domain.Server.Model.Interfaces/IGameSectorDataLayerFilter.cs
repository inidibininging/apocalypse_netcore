using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// Contains all relevant queries bound to a game sector corresponding to a data layer
    /// </summary>
    /// <typeparam name="TPlayer">Type of player</typeparam>
    /// <typeparam name="TEnemy">Type of enemy</typeparam>
    /// <typeparam name="TItem">Type of item</typeparam>
    /// <typeparam name="TProjectile">Type of projectile</typeparam>
    /// <typeparam name="TEntitiesBaseType">Base type of all models</typeparam>
    /// <typeparam name="TGeneralCharacter">Generalized type for an entity</typeparam>
    /// <typeparam name="TImageData">Type, bound to an image (ImageData, no attributes) </typeparam>
    public interface IGameSectorDataLayerFilter<TPlayer, TEnemy, TItem, TProjectile, TEntitiesBaseType, TGeneralCharacter, TImageData>
    {
        #region Queries

        IEnumerable<TEntitiesBaseType> GetAll();

        #endregion Queries
    }
}