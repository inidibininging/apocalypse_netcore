﻿using Lidgren.Network;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class NetworkCommandConnection : NetworkCommand
    {
        public NetConnection Connection { get; set; }
        public long ConnectionId => Connection == null ? 0 : Connection.RemoteUniqueIdentifier;
        //byte[] Data { get; set; }
    }
}