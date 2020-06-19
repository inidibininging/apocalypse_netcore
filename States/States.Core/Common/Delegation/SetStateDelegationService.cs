using States.Core.Common.Storage;
using States.Core.Infrastructure.Services;
using System;

namespace States.Core.Common.Delegation
{
    /// <summary>
    /// Sets a state
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type accessed</typeparam>
    public class SetStateDelegationService<TKey, TValue> : StorageDelegationBaseService<TKey, IState<TKey, TValue>>,
        IStateSetService<TKey, TValue>
    {
        public SetStateDelegationService(Func<IStorageService<TKey, IState<TKey, TValue>>> getStorage) : base(getStorage)
        {
        }

        public bool Set(IState<TKey, TValue> state, TKey identifier)
        {
            if (state == null)
                throw new ArgumentNullException(nameof(state));
            if (identifier == null)
                throw new ArgumentNullException(nameof(identifier));
            GetStorageSafe().MemoryStorage[identifier] = state;

            return true;
        }
    }
}