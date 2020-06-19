using System.Collections.Generic;

namespace States.Core.Common.Storage
{
    /// <summary>
    /// This class holds a dictionary of any values
    /// Moreover this class gives the ability of generating a new identifier by using NewIdentifier
    /// </summary>
    /// <typeparam name="TKey">The key type used for accessing the dictionary</typeparam>
    /// <typeparam name="TValue">The value type used represented by a TKey</typeparam>
    public interface IStorageService<TKey, TValue>
    {
        IDictionary<TKey, TValue> MemoryStorage { get; }

        TKey NewIdentifier();
    }
}