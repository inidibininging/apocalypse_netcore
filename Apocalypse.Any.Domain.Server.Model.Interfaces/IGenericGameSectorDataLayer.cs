using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IGenericGameSectorDataLayer
    {
        ConcurrentBag<IGenericTypeDataLayer> Layers { get; set; }

        IEnumerable<IGenericTypeDataLayer> GetLayersByName(string layerName);
        IEnumerable<IGenericTypeDataLayer> GetLayersByTypes(IEnumerable<Type> types);        
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1>();
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2>();
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3>();
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3, T4>();
        IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3, T4, T5>();
    }
}