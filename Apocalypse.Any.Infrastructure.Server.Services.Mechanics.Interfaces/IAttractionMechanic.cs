using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface IAttractionMechanic
    {
        void Update(IFullPositionHolder target, IFullPositionHolder attractionPoint, float force);
    }
}