using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Text
{
    public abstract class GenericInfoText<T>
        : Behaviour<T>,
        IVisualGameObject
        where T : IUpdateableLite, IMovableGameObject, IRotatableGameObject
    {
        private VisualText Information { get; set; }

        protected GenericInfoText(T target) : base(target)
        {
            Information = new VisualText();
        }

        public abstract string GetText();

        public override void UnloadContent()
        {
            Information.UnloadContent();
            base.UnloadContent();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Information.Text = GetText();
            Information.Draw(spriteBatch);
        }

        public virtual Vector2 GetInfoTextPositionOnScreen() => Target.Position;

        public override void Update(GameTime gameTime)
        {
            var infoTextPosition = GetInfoTextPositionOnScreen();
            Information.Position.X = infoTextPosition.X;
            Information.Position.Y = infoTextPosition.Y; //TODO: Can I pass a vector2 X and Y to a movement behaviour through an operator?

            //Information.Rotation.Rotation = ScreenService.Instance.DefaultScreenCamera.Rotation * -1;
            base.Update(gameTime);
        }
    }
}