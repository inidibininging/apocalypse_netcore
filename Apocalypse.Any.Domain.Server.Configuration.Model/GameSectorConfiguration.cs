using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Domain.Server.Sector.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Server.Configuration.Model
{
    public class GameSectorConfiguration : IGameSectorData
    {
        public string StartupScript { get; set; }
        public string StartupFunction { get;set; }
        public string BuildOperation { get; set; }
        public string RunOperation { get; set; }
        public int MaxEnemies { get; set; }
        public int MaxPlayers { get; set; }
        public List<GameSectorRoutePair> Routes  { get; set; }
        public string Tag { get; set; }
        public GameSectorStatus CurrentStatus { get; set; }
        public IGameSectorBoundaries SectorBoundaries { get; set; }
    }
}