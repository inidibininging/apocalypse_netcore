using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Transformations
{
    public class RectangleToSectorBoundaryTransformationService
    {
        public IGameSectorBoundaries ToSectorBoundary(Rectangle rectangle) => new SectorBoundary()
        {
            MinSectorX = rectangle.X,
            MinSectorY = rectangle.Y,
            MaxSectorX = rectangle.Width,
            MaxSectorY = rectangle.Height
        };
    }
}