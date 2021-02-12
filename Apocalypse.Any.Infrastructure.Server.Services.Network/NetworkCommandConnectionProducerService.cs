using System;
using System.Collections.Concurrent;
using System.Linq;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Lidgren.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Network
{
    public class NetworkCommandConnectionProducerService<TNetPeer> : IObservable<NetworkCommandConnection>
        where TNetPeer : NetPeer
    {
        private Lazy<ConcurrentQueue<NetworkCommandConnection>> NetworkCommandConnectionQueue { get; set; } = new Lazy<ConcurrentQueue<NetworkCommandConnection>>(true);
        private IInputTranslator<NetIncomingMessage, NetworkCommandConnection> IncomingMessageToNetworkCommandConnectionTranslator { get; set; }
        private NetIncomingMessageBusService<TNetPeer> NetIncomingMessageBusService { get; set; }
        private bool KeepProducing { get; set; }
        private bool KeepNotifyingConsumers { get; set; }

        public NetworkCommandConnectionProducerService(
            NetIncomingMessageBusService<TNetPeer> netIncomingMessageBusService,
            IInputTranslator<NetIncomingMessage, NetworkCommandConnection> incomingMessageToNetworkCommandConnectionTranslator
            )
        {
            if (incomingMessageToNetworkCommandConnectionTranslator == null)
                throw new ArgumentNullException(nameof(incomingMessageToNetworkCommandConnectionTranslator));
            IncomingMessageToNetworkCommandConnectionTranslator = incomingMessageToNetworkCommandConnectionTranslator;

            if (netIncomingMessageBusService == null)
                throw new ArgumentNullException(nameof(netIncomingMessageBusService));
            NetIncomingMessageBusService = netIncomingMessageBusService;
        }

        public void StartProducing()
        {
            if (KeepProducing)
                return;
            KeepProducing = true;
            Produce();
        }

        public void StopProducing()
        {
            KeepProducing = false;
        }

        private void Produce()
        {
            while (KeepProducing)
            {
                var messageChunk = NetIncomingMessageBusService.FetchMessageChunk();
                if (messageChunk != null)
                    continue;

                messageChunk.ToList().ForEach(message =>
                {
                    var translatedMessage = IncomingMessageToNetworkCommandConnectionTranslator.Translate(message);
                    if (translatedMessage == null)
                        return;
                    NetworkCommandConnectionQueue.Value.Enqueue(translatedMessage);
                });
            }
        }

        public void StartNotifyingConsumers()
        {
            if (KeepNotifyingConsumers)
                return;
            KeepNotifyingConsumers = true;
            NotifyConsumers();
        }

        public void StopNotifyingConsumers()
        {
            KeepNotifyingConsumers = false;
        }

        private void NotifyConsumers()
        {
            while (KeepNotifyingConsumers)
            {
                NetworkCommandConnection networkCommandConnection;
                if (!NetworkCommandConnectionQueue.Value.TryDequeue(out networkCommandConnection))
                    continue;
                using (var enumerator = Observers.Value.GetEnumerator())
                {
                    while (enumerator.MoveNext())
                    {
                        try
                        {
                            enumerator.Current.OnNext(networkCommandConnection);
                        }
                        catch (Exception ex)
                        {
                            enumerator.Current.OnError(ex);
                        }
                        finally
                        {
                            enumerator.Current.OnCompleted();
                        }
                    }
                }
            }
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