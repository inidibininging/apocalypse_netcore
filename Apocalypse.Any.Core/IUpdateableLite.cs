using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core
{
    /// <summary>
    /// Light version of IUpdateable (Without events)
    /// </summary>
    public interface IUpdateableLite
    {
        void Update(GameTime gameTime);
    }
}