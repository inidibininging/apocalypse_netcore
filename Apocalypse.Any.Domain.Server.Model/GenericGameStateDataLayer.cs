using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.DataLayer;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//TODO: change the namespace to Apocalypse.Any.Domain.Server.Model.Layer
namespace Apocalypse.Any.Domain.Server.Model
{
    public class GenericGameStateDataLayer : 
        GameStateDataLayer,
        IExpandedGameSectorDataLayer<
            PlayerSpaceship,
            EnemySpaceship,
            Item,
            Projectile,
            CharacterEntity,
            CharacterEntity,
            ImageData>
    {
        public ConcurrentBag<IGenericTypeDataLayer> Layers { get; set; } = new ConcurrentBag<IGenericTypeDataLayer>();

        public IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1>()
           
            => GetLayersByTypes<IGenericTypeDataLayer>(new List<Type>() { typeof(T1) });

        public IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2>()
            
        => GetLayersByTypes<IGenericTypeDataLayer>(new List<Type>() { 
            typeof(T1),
            typeof(T2),
        });

        public IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3>()
            
        => GetLayersByTypes<IGenericTypeDataLayer>(new List<Type>() {
            typeof(T1),
            typeof(T2),
            typeof(T3),
        });

        public IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3, T4>()
            
        => GetLayersByTypes<IGenericTypeDataLayer>(new List<Type>() {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
        });

        public IEnumerable<IGenericTypeDataLayer> GetLayersByType<T1, T2, T3, T4, T5>()
            
        => GetLayersByTypes<IGenericTypeDataLayer>(new List<Type>() {
            typeof(T1),
            typeof(T2),
            typeof(T3),
            typeof(T4),
            typeof(T5),
        });

        public IEnumerable<IGenericTypeDataLayer> GetLayersByTypes<IGenericTypeDataLayer>(IEnumerable<Type> types)
           
        => Layers
            .Where(l => l.GetValidTypes().Any(t => types.Any(ts => ts == t)))
            .Cast<IGenericTypeDataLayer>();

        public IEnumerable<IGenericTypeDataLayer> GetLayersByTypes(IEnumerable<Type> types)
        {
            return GetLayersByTypes(types);
        }

        IEnumerable<IGenericTypeDataLayer> IGenericGameSectorDataLayer.GetLayersByName(string layerName)
        => Layers.Where(l => l.DisplayName == layerName);
    }
}
