using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Configuration.Model
{
    public class GameServerConfiguration
    {
        public string StartingSector { get; set; }
        public string ServerPeerName { get; set; }
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public string RedisHost { get; set; }
        public int RedisPort { get; set; }
        public string SerializationAdapterType { get; set; }
        public double ServerUpdateInSeconds { get; set; }

        public List<GameSectorConfiguration> SectorConfigurations { get; set; }
    }
}
