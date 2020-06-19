using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Utilities
{
    public class SplashScreen : GameScreen
    {
        public Image Image;

        public override void LoadContent(ContentManager manager)
        {
            base.LoadContent(manager);
            Image.LoadContent(manager);
        }

        public override void UnloadContent()
        {
            base.UnloadContent();
            Image.UnloadContent();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Image.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Image.Draw(spriteBatch);
        }
    }
}