using Apocalypse.Any.Core.Collision;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.Core.Services
{
    /// <summary>
    /// Self explanatory. A "service" that can register objects that share the ICollidable interface.
    /// The main purpose of this service is to calculate which objects collide with each other and pass that to them
    /// </summary>
    public class RectangleCollisionDetectionService : GameObject
    {
        private bool CollisionCycleEnded { get; set; }

        public RectangleCollisionDetectionService()
        {
            CollisionCycleEnded = true;
        }

        /// <summary>
        /// Registers an object that can be added to this service.
        /// TODO: Must be removed from here if its marked for deletion
        /// </summary>
        /// <param name="name"></param>
        /// <param name="gameObject"></param>
        public void RegisterGameObject(string name, ICollidable gameObject)
        {
            //Console.WriteLine(name);
            Add(name, gameObject);
        }

        /// <summary>
        /// Registers all game objects of a parent gameObject for collision detection
        /// </summary>
        /// <param name="gameObject"></param>
        public void RegisterAllCollidables(GameObject gameObject) =>
            gameObject.ForEach
            (
                subGameObject =>
                {
                    if (subGameObject is ICollidable)
                        RegisterGameObject($"{Guid.NewGuid().ToString()}_{gameObject.GetType().Name}", (ICollidable)subGameObject);
                }
            );

        private void HandleCollisions(ICollidable collidable, IEnumerable<ICollidable> collidables)
        {
            //return if game object is already disposed or marked to be removed
            if ((collidable as IDeletableGameObject)?.MarkedForDeletion == true)
                return;

            //calculate rectangle of subject
            var collidableRect = collidable.GetCurrentRectangle();

            //check if object can propagate the OnCollision method to any propable game object
            collidables
            .ToList()
            .ForEach
            (
                propableSubject =>
                {
                    //kick irrelevant game objects
                    if ((propableSubject as IDeletableGameObject)?.MarkedForDeletion == true)
                        return;
                    if (propableSubject.Equals(collidable))
                        return;
                    var subjectRect = propableSubject.GetCurrentRectangle();

                    if (subjectRect == Rectangle.Empty)
                        return;
                    else
                    //invoke the OnCollision method
                        if (collidableRect
                        .Intersects(subjectRect))
                        propableSubject.OnCollision(collidable);
                }
            );
        }

        public override void Update(GameTime time)
        {
            CollisionCycleEnded = false;

            var collidables = Values.OfType<ICollidable>().ToList();

            //single/dual core handling vs. cores >= 2
            if (Environment.ProcessorCount <= 2)
            {
                collidables.ForEach(c => HandleCollisions(c, collidables));
            }
            else
            {
                var parallelAction = Parallel.ForEach(
                collidables, new ParallelOptions() { MaxDegreeOfParallelism = 1 },
                    c => HandleCollisions(c, collidables));
                while (!parallelAction.IsCompleted)
                {
                }
            }

            CollisionCycleEnded = true;
        }
    }
}