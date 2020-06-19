using System;
using System.Collections.Generic;

namespace States.Core.Common.Storage
{
    /// <summary>
    /// Basic implementation of an object that holds a dictionary.
    /// The objects goal is to have the ability of drawing and setting information from the property MemoryStorage
    /// This object is used for storing other objects in memory
    /// </summary>
    /// <typeparam name="TKey">The key type used for lexing through a dictionary</typeparam>
    /// <typeparam name="TValue">The value type used in the dictionary</typeparam>
    public class InMemoryStateStorageService<TKey, TValue> : IStorageService<TKey, TValue>
    {
        /// <summary>
        /// Temporary memory object for holding information in the memory
        /// </summary>
        /// <value>null</value>
        public IDictionary<TKey, TValue> MemoryStorage { get; private set; }

        public InMemoryStateStorageService(IDictionary<TKey, TValue> storageData)
        {
            if (storageData == null)
                throw new ArgumentNullException(nameof(storageData));
            MemoryStorage = storageData;
        }

        /// <summary>
        /// Gives a new generated identifier back.Mainly used for generating new keys of type TKey
        /// </summary>
        /// <returns>Generated key</returns>
        public virtual TKey NewIdentifier() => default(TKey);
    }
}