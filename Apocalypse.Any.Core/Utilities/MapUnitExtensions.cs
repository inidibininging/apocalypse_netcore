using Apocalypse.Any.Core.Map.Flat;

namespace Apocalypse.Any.Core.Utilities
{
    public static class MapUnitExtensions
    {
        public static bool HasSamePosition(this MapUnit val, MapUnit other)
        {
            return val.X == other.X && val.Y == other.Y;
        }
    }
}