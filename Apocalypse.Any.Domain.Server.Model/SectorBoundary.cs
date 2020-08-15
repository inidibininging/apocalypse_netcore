using Apocalypse.Any.Domain.Server.Model.Interfaces;

namespace Apocalypse.Any.Domain.Server.Model
{
    /// <summary>
    /// Represents a rectangular sector
    /// TODO: there should a vice-versa conversion of the this class into a rectangle
    /// </summary>
    public class SectorBoundary : IGameSectorBoundaries
    {
        public int MinSectorX { get; set; }
        public int MaxSectorX { get; set; }
        public int MinSectorY { get; set; }
        public int MaxSectorY { get; set; }
    }
}