using System.Collections.Concurrent;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// Contains all relevant data bound to a game sector
    /// </summary>
    /// <typeparam name="TPlayer">Type of player</typeparam>
    /// <typeparam name="TEnemy">Type of enemy</typeparam>
    /// <typeparam name="TItem">Type of item</typeparam>
    /// <typeparam name="TProjectile">Type of projectile</typeparam>
    /// <typeparam name="TEntitiesBaseType">Base type of all models</typeparam>
    /// <typeparam name="TGeneralCharacter">Generalized type for an entity</typeparam>
    /// <typeparam name="TImageData">Type, bound to an image (ImageData, no attributes) </typeparam>
    public interface IGameSectorDataLayer<TPlayer, TEnemy, TItem, TProjectile, TEntitiesBaseType, TGeneralCharacter, TImageData>
    {
        #region Entities

        ConcurrentBag<TPlayer> Players { get; set; }
        ConcurrentBag<TEnemy> Enemies { get; set; }
        ConcurrentBag<TProjectile> Projectiles { get; set; }

        ConcurrentBag<TItem> Items { get; set; }
        ConcurrentBag<TEntitiesBaseType> GeneralCharacter { get; set; }
        ConcurrentBag<TImageData> ImageData { get; set; }
        ConcurrentBag<TItem> PlayerItems { get; set; }
        

        #endregion Entities
    }
}