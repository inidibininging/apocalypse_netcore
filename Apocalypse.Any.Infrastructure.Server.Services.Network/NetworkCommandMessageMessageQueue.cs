using System;
using System.Linq;
using System.Collections.Concurrent;
using Apocalypse.Any.Domain.Common.Model.Network;
using System.Threading.Tasks;

namespace Apocalypse.Any.Infrastructure.Server.Services.Network
{
    public class NetworkCommandMessageQueue 
        : IObserver<NetworkCommandConnection>,
        IObservable<NetworkCommandConnection>
    {
        private Func<NetworkCommandConnection, bool> Predicate { get; }
        public bool IgnoresErrors { get; }
        public NetworkCommandMessageQueue(
            Func<NetworkCommandConnection, bool> predicate,
            bool ignoresErrors = false)
        {
            Predicate = predicate;
            IgnoresErrors = ignoresErrors;
        }

        public async void OnCompleted()
        {
            await Task.Run(() => {
                foreach(var observer in Observers.Value)
                    observer?.OnCompleted();
            });
        }

        public async void OnError(Exception error)
        {
            if(IgnoresErrors)
                return;
            await Task.Run(() => {
                foreach(var observer in Observers.Value)
                    observer?.OnError(error);
            });
        }

        public async void OnNext(NetworkCommandConnection value)
        {
            if(!Predicate(value))
                return;
            await Task.Run(() => {
                foreach(var observer in Observers.Value)
                    observer?.OnNext(value);
            });
        }

        private Lazy<ConcurrentBag<IObserver<NetworkCommandConnection>>> Observers { get; set; } = new Lazy<ConcurrentBag<IObserver<NetworkCommandConnection>>>(true);

        public IDisposable Subscribe(IObserver<NetworkCommandConnection> observer)
        {
            var wrappedObserver = new NetworkCommandConnectionConsumerWrapperService(observer);
            Observers.Value.Add(wrappedObserver);
            return wrappedObserver;
        }
        
    }
}
