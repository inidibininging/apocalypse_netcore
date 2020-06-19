using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Services;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Client.Mechanics
{
    /// <summary>
    /// This behaviour prevents a game object from "escaping" the game screen
    /// </summary>
    public class AsteroidScreenBehaviour : Behaviour<IImage>
    {
        public AsteroidScreenBehaviour(IImage gameObject) : base(gameObject)
        {
        }

        public override void Update(GameTime gameTime)
        {
            //FIX
            return;
            var movement = Target.Position;

            var posX = ScreenService.Instance.DefaultScreenCamera.Position.X + ScreenService.Instance.Resolution.X;
            var posY = ScreenService.Instance.DefaultScreenCamera.Position.Y + ScreenService.Instance.Resolution.Y;

            if (movement.X >= posX)
            {
                movement.X = movement.X - ScreenService.Instance.Resolution.X;
            }
            if (movement.X <= 0)
            {
                movement.X = posX - movement.X - 10;
            }
            if (movement.Y >= posY)
            {
                movement.Y = movement.Y - posY;
            }
            if (movement.Y <= 0)
            {
                movement.Y = posY - movement.Y - 10;
            }
            Target.Position = movement;
            base.Update(gameTime);
        }
    }
}