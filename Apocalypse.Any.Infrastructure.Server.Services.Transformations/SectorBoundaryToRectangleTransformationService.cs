using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations
{
    public class SectorBoundaryToRectangleTransformationService
    {
        public Rectangle ToRectangle(IGameSectorBoundaries sectorBoundary) => new Rectangle(
               sectorBoundary.MinSectorX,
               sectorBoundary.MinSectorY,
               sectorBoundary.MaxSectorX,
               sectorBoundary.MaxSectorY);
    }
}