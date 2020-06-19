using States.Core.Common.Storage;
using States.Core.Infrastructure.Services;
using System;

namespace States.Core.Common.Delegation
{
    /// <summary>
    /// Allocates a state
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class NewStateDelegationService<TKey, TValue> : StorageDelegationBaseService<TKey, IState<TKey, TValue>>,
        IStateNewService<TKey, TValue>
    {
        public NewStateDelegationService(Func<IStorageService<TKey, IState<TKey, TValue>>> getStorage) : base(getStorage)
        {
        }

        public TKey New(IState<TKey, TValue> state)
        {
            var storage = GetStorageSafe();
            var key = GetStorageSafe().NewIdentifier();
            storage.MemoryStorage.Add(key, state);
            return key;
        }

        public TKey New(TKey identifier, IState<TKey, TValue> state)
        {
            var storage = GetStorageSafe();
            GetStorageSafe().MemoryStorage.Add(identifier, state);
            return identifier;
        }
    }
}