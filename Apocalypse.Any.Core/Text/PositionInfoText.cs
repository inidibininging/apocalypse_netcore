using Apocalypse.Any.Core.Behaviour;
using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Text
{
    public class PositionInfoText : Behaviour<CollidableGameObject<AnimatedImage>>, IVisualGameObject
    {
        public VisualText Information { get; set; }

        public PositionInfoText(CollidableGameObject<AnimatedImage> target) : base(target)
        {
            Information = "_info_";
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            string fullText = $@"
X:{Target.CurrentImage.Position.X}
Y:{Target.CurrentImage.Position.Y}
Rotation:{Target.CurrentImage.Rotation.Rotation}
Colliding:{Target.Colliding}";
            Information.Text = fullText;
            Information.Draw(spriteBatch);
        }

        public override void Update(GameTime gameTime)
        {
            Information.Position = Target.CurrentImage.Position;

            //Information.Movement.Y = Target.Movement.Y + 50;
            base.Update(gameTime);
        }
    }
}