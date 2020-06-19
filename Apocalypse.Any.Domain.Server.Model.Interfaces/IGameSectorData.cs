namespace Apocalypse.Any.Domain.Server.Model.Interfaces
{
    public interface IGameSectorData
    {
        string Tag { get; set; }
        GameSectorStatus CurrentStatus { get; set; }
        int MaxEnemies { get; set; }
        int MaxPlayers { get; set; }

        IGameSectorBoundaries SectorBoundaries { get; set; }
    }
}