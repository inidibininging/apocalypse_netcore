using Apocalypse.Any.Core.Collision;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class RectangleCollisionMechanic : IRectangleCollisionMechanic
    {
        public RectangleCollisionMechanic()
        {
        }

        public void HandleCollisions(ICollidable collidable, IEnumerable<ICollidable> collidableObjects)
        {
            var collidableRect = collidable.GetCurrentRectangle();
            collidableObjects
            .ToList()
            .ForEach
            (
                propableSubject =>
                {
                    if (propableSubject.Equals(collidable) || propableSubject == collidable)
                        return;

                    var subjectRect = propableSubject.GetCurrentRectangle();
                    if (subjectRect == Rectangle.Empty)
                        return;
                    else
                        if (collidableRect.Intersects(subjectRect))
                        propableSubject.OnCollision(collidable);
                }
            );
        }

        public IEnumerable<ICollidable> Update(IEnumerable<ICollidable> collidableObjects)
        {
            var parallelAction = Parallel.ForEach(
                    collidableObjects, new ParallelOptions() { MaxDegreeOfParallelism = 1 },
                        c =>
                        {
                            HandleCollisions(c, collidableObjects);
                        });
            while (!parallelAction.IsCompleted)
            {
            }
            return collidableObjects;
        }
    }
}