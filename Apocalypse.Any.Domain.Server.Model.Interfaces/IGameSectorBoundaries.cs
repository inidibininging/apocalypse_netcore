namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    /// <summary>
    /// Contains sector boundaries
    /// </summary>
    public interface IGameSectorBoundaries
    {
        int MinSectorX { get; set; }
        int MaxSectorX { get; set; }
        int MinSectorY { get; set; }
        int MaxSectorY { get; set; }
    }
}