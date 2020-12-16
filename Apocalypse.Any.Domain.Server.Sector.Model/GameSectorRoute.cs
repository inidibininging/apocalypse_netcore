namespace Apocalypse.Any.Domain.Server.Sector.Model
{
    /// <summary>
    /// Describes where something heads to in a sector.
    /// </summary>
    public class GameSectorRoute
    {
        public GameSectorTrespassingDirection Trespassing { get; set; }

        public int GameSectorTag { get; set; }
    }
}