using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Behaviour
{
    /// <summary>
    /// This class is the base class for all behaviours in game. Behaviours are game objects that hold a certain game logic of a game object outside of the object.
    /// This logic can be then attached to every object of type IGameObject
    /// This is a decorator pattern with extra sauce
    /// </summary>
	public class Behaviour : Behaviour<IUpdateableLite> //todo: change name to Attachable
    {
        public Behaviour(IUpdateableLite target) : base(target)
        {
        }
    }

    /// <summary>
    /// This class is the base class for all behaviours in game. Behaviours are game objects that hold a certain game logic of a game object outside of the object.
    /// This logic can be then attached to every object of type specified (T)
    /// This is a decorator pattern with extra sauce
    /// </summary>
    public class Behaviour<T> : GameObject, IBehaviour<T>
        where T : IUpdateableLite
    {
        /// <summary>
        /// Attaches the object here.
        /// </summary>
        /// <param name="target"></param>
		protected Behaviour(T target)
        {
            Attach(target);
        }

        protected T Target { get; set; }

        public void Attach(T target)
        {
            Target = target;
        }

        public override void UnloadContent()
        {
            Target = default(T);
            base.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            this.ForEach(obj => obj.Update(gameTime));
        }
    }
}