using System;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Echse.Net.Serialization;
using Lidgren.Network;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network
{
    public class NetworkOutgoingMessageCreator<TNetPeer>
        where TNetPeer : NetPeer
    {
        private TNetPeer Peer { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }
        
        public NetworkOutgoingMessageCreator(TNetPeer peer, IByteArraySerializationAdapter serializationAdapter)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));
            Peer = peer;
            if (serializationAdapter == null)
                throw new ArgumentNullException(nameof(serializationAdapter));
            SerializationAdapter = serializationAdapter;
        }
        
        public NetOutgoingMessage CreateMessage<T>(T instanceToSend)
        {
            var message = Peer.CreateMessage();
            message.Write(SerializationAdapter.SerializeObject(instanceToSend));
            return message;
        }
    }
}