using System;

namespace States.Core.Common.Storage
{
    /// <summary>
    /// Basic storage accessor.
    /// </summary>
    /// <typeparam name="TKey">The key type</typeparam>
    /// <typeparam name="TValue">The value type accessed</typeparam>
    public abstract class StorageDelegationBaseService<TKey, TValue>
    {
        public Func<IStorageService<TKey, TValue>> GetStorage { get; private set; }

        public StorageDelegationBaseService(Func<IStorageService<TKey, TValue>> getStorage)
        {
            GetStorage = getStorage ?? throw new ArgumentNullException(nameof(GetStorage));
        }

        protected IStorageService<TKey, TValue> GetStorageSafe()
        {
            var storage = GetStorage();
            if (storage == null)
                throw new ArgumentNullException("Storage object is set to null");
            return storage;
        }
    }
}