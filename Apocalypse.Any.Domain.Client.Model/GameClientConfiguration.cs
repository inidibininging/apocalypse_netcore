using Apocalypse.Any.Domain.Common.Model.Network;
using System;

namespace Apocalypse.Any.Domain.Client.Model
{
    public class GameClientConfiguration
    {
        public string ServerPeerName { get; set; }
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public bool WithLocalServer { get; set; }
        public string LocalServerPath { get; set; }
        public UserData User { get; set; }
        public ScreenData Screen { get; set; }
        public string SerializationAdapterType { get; set; }
        public SpaceBackgroundElementsConfiguration Background { get; set; }
    }
}
