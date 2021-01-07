﻿namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class NetworkCommand : INetworkCommand<byte, string, byte[]>
    {
        public byte CommandName { get; set; }
        public string CommandArgument { get; set; }
        public byte[] Data { get; set; }
    }
}