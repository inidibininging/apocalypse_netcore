using Microsoft.Xna.Framework.Graphics;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// This interface is for game objects that can be displayed visually.
    /// </summary>
    public interface IVisualGameObject : IGameObject
    {
        void Draw(SpriteBatch spriteBatch);
    }
}