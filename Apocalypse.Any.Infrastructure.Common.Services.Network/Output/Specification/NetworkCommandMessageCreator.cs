using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Output.Specification
{
    public class NetworkCommandMessageCreator : INetOutgoingMessageCreator<NetworkCommand>
    {
        public ISerializationAdapter SerializationAdapter { get; }
        public NetworkCommandMessageCreator(ISerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter;
        }

        public NetOutgoingMessage CreateMessage(NetworkCommand instance, NetPeer netPeer)
        {
            return netPeer.CreateMessage(SerializationAdapter.SerializeObject(instance));
        }
    }
}
