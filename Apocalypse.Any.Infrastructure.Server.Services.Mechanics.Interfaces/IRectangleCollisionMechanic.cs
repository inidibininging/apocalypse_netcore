using Apocalypse.Any.Core.Collision;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface IRectangleCollisionMechanic
    {
        void HandleCollisions(ICollidable collidable, IEnumerable<ICollidable> collidableObjects);

        IEnumerable<ICollidable> Update(IEnumerable<ICollidable> collidableObjects);
    }
}