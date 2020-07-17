using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// Contains all relevant data bound to a game sector.    
    /// This interface expands the main types with a reflection based generic part.
    /// </summary>
    /// <typeparam name="TPlayer">Type of player</typeparam>
    /// <typeparam name="TEnemy">Type of enemy</typeparam>
    /// <typeparam name="TItem">Type of item</typeparam>
    /// <typeparam name="TProjectile">Type of projectile</typeparam>
    /// <typeparam name="TEntitiesBaseType">Base type of all models</typeparam>
    /// <typeparam name="TGeneralCharacter">Generalized type for an entity</typeparam>
    /// <typeparam name="TImageData">Type, bound to an image (ImageData, no attributes) </typeparam>
    public interface IExpandedGameSectorDataLayer<TPlayer, TEnemy, TItem, TProjectile, TEntitiesBaseType, TGeneralCharacter, TImageData> 
        : IGameSectorDataLayer<TPlayer, TEnemy, TItem, TProjectile, TEntitiesBaseType, TGeneralCharacter, TImageData>,
        IGenericGameSectorDataLayer
    {

    }
}
