using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network
{
    public class NetIncomingMessageBusService<TNetPeer> :
        INetIncomingMessageBusService
        where TNetPeer : NetPeer
    {
        private TNetPeer Peer { get; set; }

        public NetIncomingMessageBusService(TNetPeer peer)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));
            Peer = peer;
        }

        public Task<List<NetIncomingMessage>> FetchMessageChunkAsync()
        {
            return new Task<List<NetIncomingMessage>>(() =>
            {
                return FetchMessageChunk();
            });
        }

        public List<NetIncomingMessage> FetchMessageChunk()
        {
            List<NetIncomingMessage> fetchedMessages = new List<NetIncomingMessage>();
            var fetchMessageResult = Peer.ReadMessages(fetchedMessages);
            return fetchedMessages;
        }
    }
}