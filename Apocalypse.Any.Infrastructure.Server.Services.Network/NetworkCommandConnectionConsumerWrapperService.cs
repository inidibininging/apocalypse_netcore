using Apocalypse.Any.Domain.Common.Model.Network;
using System;

namespace Apocalypse.Any.Core.Network.Server.Services
{
    public class NetworkCommandConnectionConsumerWrapperService : IObserver<NetworkCommandConnection>, IDisposable
    {
        private IObserver<NetworkCommandConnection> Observer { get; set; }

        public NetworkCommandConnectionConsumerWrapperService(IObserver<NetworkCommandConnection> observer)
        {
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));
            Observer = observer;
        }

        public void Dispose()
        {
            Observer = null;
        }

        public void OnCompleted()
        {
            Observer?.OnCompleted();
        }

        public void OnError(Exception error)
        {
            Observer?.OnError(error);
        }

        public void OnNext(NetworkCommandConnection value)
        {
            Observer?.OnNext(value);
        }
    }
}