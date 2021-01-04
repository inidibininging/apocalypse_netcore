using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces
{
    public interface INetOutgoingMessageCreator<T>
    {
        NetOutgoingMessage CreateMessage(T instance, NetPeer netPeer);
    }
}
