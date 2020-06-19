using States.Core.Common.Storage;
using States.Core.Infrastructure.Services;
using System;

namespace States.Core.Common.Delegation
{
    /// <summary>
    /// Gets a state through a Func.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type accessed</typeparam>
    public class GetStateDelegationService<TKey, TValue> : StorageDelegationBaseService<TKey, IState<TKey, TValue>>,
        IStateGetService<TKey, TValue>
    {
        public GetStateDelegationService(Func<IStorageService<TKey, IState<TKey, TValue>>> getStorage) : base(getStorage)
        {
        }

        public IState<TKey, TValue> Get(TKey identifier)
        {
            if (identifier == null)
                throw new ArgumentNullException(nameof(identifier));
            return GetStorage().MemoryStorage[identifier];
        }

        public bool HasState(TKey identifier)
        {
            return GetStorage().MemoryStorage.ContainsKey(identifier);
        }
    }
}