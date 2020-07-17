using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Server.Model;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    /// <summary>
    /// Provides a generic in memory data layer
    /// </summary>
    public class GenericInMemoryDataLayer<TData> : CheckedWithReflectionGameStateDataLayer
        where TData : class, IIdentifiableModel
    {
        /// <summary>
        /// If true, every AddSafe will avoid items with the same id.
        /// By default, this will be set to false
        /// </summary>
        protected bool OnlyUniques { get; private set; }

        public GenericInMemoryDataLayer(bool onlyUniques = false)
        {
            OnlyUniques = onlyUniques;
        }

        private ConcurrentBag<TData> Data { get; set; } = new ConcurrentBag<TData>();
        public override bool CanUse<T>(T instance)
        {
            return instance != null && 
                   CanUseByTType<T, TData>() &&
                   OnlyUniques ? !Data.Any(item => item.Id == (instance as TData)?.Id) : true;
        }

        public override List<Type> GetValidTypes()
        {
            return new List<Type> { typeof(TData) };
        }

        protected override void AddSafe<T>(T item)
        {
            Data.Add(item as TData);
        }

        protected override IEnumerable<T> AsEnumerableSafe<T>()
        {
            return Data.Cast<T>();
        }
    }
}
