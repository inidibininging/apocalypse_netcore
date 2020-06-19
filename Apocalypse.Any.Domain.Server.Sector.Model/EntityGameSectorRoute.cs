using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Domain.Server.Sector.Model
{
    public class EntityGameSectorRoute : GameSectorRoute
    {
        public Vector2 Position { get; set; }
        public string LoginToken { get; set; }
    }
}