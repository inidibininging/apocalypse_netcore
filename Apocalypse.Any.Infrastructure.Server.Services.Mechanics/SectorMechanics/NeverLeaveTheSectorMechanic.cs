using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics
{
    /// <summary>
    /// This behaviour prevents a game object from "escaping" the game screen
    /// </summary>
    public class NeverLeaveTheSectorMechanic :
        INeverLeaveTheScreenMechanic
    {
        public NeverLeaveTheSectorMechanic()
        {
        }

        public void Update(IFullPositionHolder target, IGameSectorBoundaries sectorBoundary)
        {
            if (target.Position.X >= sectorBoundary.MaxSectorX)
            {
                target.Position.X = target.Position.X - sectorBoundary.MaxSectorX + 10;
            }
            if (target.Position.X <= sectorBoundary.MinSectorX)
            {
                target.Position.X = target.Position.X + sectorBoundary.MaxSectorX - 10;
            }
            if (target.Position.Y >= sectorBoundary.MaxSectorY)
            {
                target.Position.Y = sectorBoundary.MaxSectorY - target.Position.Y + 10;
            }
            if (target.Position.Y <= sectorBoundary.MinSectorY)
            {
                target.Position.Y = sectorBoundary.MaxSectorY + target.Position.Y - 10;
            }
        }
    }
}