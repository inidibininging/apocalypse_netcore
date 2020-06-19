using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface IRandomRotationMechanic
    {
        IFullPositionHolder Update(IFullPositionHolder target);
    }
}