﻿using Apocalypse.Any.Core.Model;
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
        where TData : class 
    {
        /// <summary>
        /// If true, every AddSafe will avoid items with the same id.
        /// By default, this will be set to false
        /// </summary>
        protected bool OnlyUniques { get; private set; }

        public GenericInMemoryDataLayer(string name, bool onlyUniques = false) : base(name)
        {
            OnlyUniques = onlyUniques;
        }

        private ConcurrentBag<TData> Data { get; set; } = new ConcurrentBag<TData>();
        public override bool CanUse<T>(T instance)
        {
            var instanceId = instance?.GetType().GetProperty("Id");
            if (instanceId == null)
                return false;
            
            return !CanUseByTType<T, TData>() || !OnlyUniques || Data.All(item =>
            {
                var idItem = item.GetType().GetProperty("Id");
                
                if (idItem == null)
                    return false;
                //fixed for now with reflection. needs to be done without it
                return idItem.GetValue(instance) != instanceId.GetValue(instance);
            });
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

        protected override bool UpdateEnumerable<T>(IEnumerable<T> items)
        {
            if (!items.All(item => CanUse(item)))
                return false;
            Data = new ConcurrentBag<TData>(items.Cast<TData>());	    
            return true;
        }
    }
}
