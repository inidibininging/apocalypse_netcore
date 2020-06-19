using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Model
{
    public class GameServerConfiguration
    {
        public string StartupScript { get; set; }
        public string StartupFunction { get;set; }
        public string BuildOperation { get; set; }
        public string RunOperation { get; set; }
        public int MaxEnemies { get; set; }
        public int MaxPlayers { get; set; }
        public string StartingSector { get; set; }
        public string ServerPeerName { get; set; }
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public string RedisHost { get; set; }
        public int RedisPort { get; set; }
        public string SerializationAdapterType { get; set; }
        public double ServerUpdateInSeconds { get; set; }
        public int SectorXSize { get; set; }
        public int SectorYSize { get; set; }
    }
}
