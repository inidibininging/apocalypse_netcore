using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Server.Model.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface INeverLeaveTheScreenMechanic
    {
        void Update(IFullPositionHolder target, IGameSectorBoundaries sectorBoundary);
    }
}