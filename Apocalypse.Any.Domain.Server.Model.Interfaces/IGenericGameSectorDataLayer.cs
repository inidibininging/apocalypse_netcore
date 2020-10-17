using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// Interface for providing generic data layers of IGenericTypeDataLayer
    /// </summary>
    public interface IGenericGameSectorDataLayer
    {
        ConcurrentBag<IGenericTypeDataLayer> Layers { get; set; }

        /// <summary>
        /// Shortcut method for returning instances of "Layers", based on the layer name provided
        /// </summary>
        /// <param name="layerName">The layer name of all IGenericTypeDataLayers </param>
        /// <returns></returns>
        IEnumerable<IGenericTypeDataLayer> GetLayersByName(string layerName);

        /// <summary>
        /// Shortcut for returning instances of "Layers" based on all the types provided
        /// </summary>
        /// <param name="types">A list of all types accepted by all layers of "Layers"</param>
        /// <returns>All valid layers of IGenericTypeDataLayers in "Layers"</returns>
        IEnumerable<IGenericTypeDataLayer> GetLayersByTypes(IEnumerable<Type> types);

        /// <summary>
        /// Shortcut for returning instances of "Layers" that accept the type T1
        /// </summary>
        /// <typeparam name="T1">The type that can be accepted</typeparam>
        /// <returns>All valid layers of IGenericTypeDataLayers in "Layers"</returns>
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1>();

        /// <summary>
        /// Shortcut for returning instances of "Layers" that accept the type T1
        /// </summary>
        /// <typeparam name="T1">The type that can be accepted</typeparam>
        /// <returns>All valid layers of IGenericTypeDataLayers in "Layers"</returns>
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2>();

        /// <summary>
        /// Shortcut for returning instances of "Layers" that accept the type T1, T2
        /// </summary>
        /// <typeparam name="T1">The type that can be accepted</typeparam>
        /// <returns>All valid layers of IGenericTypeDataLayers in "Layers"</returns>
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3>();

        /// <summary>
        /// Shortcut for returning instances of "Layers" that accept the type T1, T2, T3
        /// </summary>
        /// <typeparam name="T1">The type that can be accepted</typeparam>
        /// <returns>All valid layers of IGenericTypeDataLayers in "Layers"</returns>
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3, T4>();

        /// <summary>
        /// Shortcut for returning instances of "Layers" that accept the type T1, T2, T3, T4
        /// </summary>
        /// <typeparam name="T1">The type that can be accepted</typeparam>
        /// <returns>All valid layers of IGenericTypeDataLayers in "Layers"</returns>
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3, T4, T5>();
    }
}