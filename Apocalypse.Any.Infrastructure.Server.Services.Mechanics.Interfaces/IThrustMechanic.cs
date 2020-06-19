using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface IThrustMechanic
    {
        float BasicAcceleration { get; set; }

        IFullPositionHolder Update(IFullPositionHolder target, float speedFactor);
    }
}