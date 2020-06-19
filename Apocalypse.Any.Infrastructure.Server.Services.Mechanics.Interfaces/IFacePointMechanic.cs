using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface IFacePointMechanic
    {
        void Update(IFullPositionHolder target, IFullPositionHolder attractionPoint);
    }
}