using Apocalypse.Any.Domain.Server.Model.Interfaces;

namespace Apocalypse.Any.Domain.Server.Model
{
    public class SectorBoundary : IGameSectorBoundaries
    {
        public int MinSectorX { get; set; }
        public int MaxSectorX { get; set; }
        public int MinSectorY { get; set; }
        public int MaxSectorY { get; set; }
    }
}