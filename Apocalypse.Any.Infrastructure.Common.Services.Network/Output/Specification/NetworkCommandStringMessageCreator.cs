using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Echse.Net.Domain;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Output.Specification
{
    public class NetworkCommandStringMessageCreator
        : INetOutgoingMessageCreator<NetworkCommand>
    {
        public IStringSerializationAdapter SerializationAdapter { get; }
        public NetworkCommandStringMessageCreator(IStringSerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter;
        }

        public NetOutgoingMessage CreateMessage(NetworkCommand instance, NetPeer netPeer)
        {
            return netPeer.CreateMessage(SerializationAdapter.SerializeObject(instance));
        }
    }
}
