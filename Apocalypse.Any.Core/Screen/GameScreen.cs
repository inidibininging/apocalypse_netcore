using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Utilities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Screen
{
    /// <summary>
    /// This class is the base class for all screens in a game.
    /// The only purpouse of this class is finding all objects that are visual and trying to execute the draw method for them
    /// </summary>
    public class GameScreen : GameObject, IVisualGameObject, IGameScreen
    {
        /// <summary>
        /// Draws all objects added to the game screen of type IVisualGameObject
        /// </summary>
        /// <param name="spriteBatch"></param>
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            foreach (var gameObject in this)
            {
                if (gameObject.Value.GetType().GetInterface(nameof(IVisualGameObject)) != null)
                {
                    ((IVisualGameObject)gameObject.Value).Draw(spriteBatch);
                }
            }
        }

        public override void Update(GameTime time)
        {
            UpdateGameTime = time;
            ForEach(obj => obj.Update(time));
        }

        public IImage CursorImage { get; set; }
        public GameTime UpdateGameTime { get; private set; }
    }
}