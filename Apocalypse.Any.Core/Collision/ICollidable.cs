using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Collision
{
    /// <summary>
    /// Describes a game object that exposes his visual rectangle.
    /// The purpose of this interface is to communicate the collision status of the game object to all related objects.
    /// If you want to know any information of the colliding object, you will have implement a some kind of observer pattern.
    /// </summary>
    public interface ICollidable : IGameObjectDictionary
    {
        Rectangle GetCurrentRectangle();

        void OnCollision(ICollidable collidable); //intrisic

        bool Colliding { get; }
    }
}