using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using Echse.Net.Serialization;
using Echse.Net.Domain;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network
{
    public class NetOutgoingMessageBusService<TNetPeer>
        where TNetPeer : NetPeer
    {
        private TNetPeer Peer { get; set; }
        private IByteArraySerializationAdapter SerializationAdapter { get; }

        public NetOutgoingMessageBusService(TNetPeer peer, IByteArraySerializationAdapter serializationAdapter)
        {
            if (peer == null)
                throw new ArgumentNullException(nameof(peer));
            Peer = peer;
            if (serializationAdapter == null)
                throw new ArgumentNullException(nameof(serializationAdapter));
            SerializationAdapter = serializationAdapter;
        }

        private NetOutgoingMessage CreateMessage<T>(T instanceToSend)
        {
            var msg = Peer.CreateMessage();
            msg.Write(SerializationAdapter.SerializeObject(instanceToSend));
            return msg;
        }

        public NetSendResult SendToClient<T>(byte commandName, T instanceToSend, NetDeliveryMethod netDeliveryMethod, int sequenceChannel, NetConnection netConnection)
        {
            if (netDeliveryMethod == 0)
                netDeliveryMethod = NetDeliveryMethod.ReliableOrdered;
            return netConnection.SendMessage(
                    CreateMessage(
                        new NetworkCommand()
                        {
                            CommandName = commandName,
                            CommandArgument = typeof(T).FullName,
                            Data = SerializationAdapter.SerializeObject(instanceToSend),
                        }
                ),
                netDeliveryMethod,
                sequenceChannel); //TODO: Assign a T to a channel by using a Dictionary<Type,int>
        }

        public List<(long remoteConnectionId, NetSendResult)> Broadcast<T>(byte commandName, T instanceToSend, NetDeliveryMethod netDeliveryMethod, int sequenceChannel){
            if(Peer.Connections.Count == 0)
                return Array.Empty<(long remoteConnectionId, NetSendResult)>().ToList();
            return Peer.Connections.ConvertAll(connection => (connection.RemoteUniqueIdentifier, SendToClient<T>(commandName, instanceToSend, netDeliveryMethod, sequenceChannel, connection)));
        }
    }
}