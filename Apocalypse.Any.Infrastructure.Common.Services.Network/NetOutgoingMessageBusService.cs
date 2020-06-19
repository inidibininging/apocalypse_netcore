using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;
using System;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network
{
    public class NetOutgoingMessageBusService<TNetPeer>
        where TNetPeer : NetPeer
    {
        private TNetPeer Peer { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }

        public NetOutgoingMessageBusService(TNetPeer peer, ISerializationAdapter serializationAdapter)
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
            return Peer.CreateMessage(SerializationAdapter.SerializeObject(instanceToSend));
        }

        public NetSendResult SendToClient<T>(string commandName, T instanceToSend, NetConnection netConnection)
        {
            return netConnection.SendMessage(
                    CreateMessage(
                        new NetworkCommand()
                        {
                            CommandName = commandName,
                            CommandArgument = typeof(T).FullName,
                            Data = SerializationAdapter.SerializeObject(instanceToSend),
                        }
                ),
                NetDeliveryMethod.ReliableOrdered,
                0); //TODO: Assign a T to a channel by using a Dictionary<Type,int>
        }
    }
}