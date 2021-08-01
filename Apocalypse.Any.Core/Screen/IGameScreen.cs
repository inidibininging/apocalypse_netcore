using Apocalypse.Any.Core.Drawing;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Screen
{
    public interface IGameScreen : IVisualGameObject, IGameObjectDictionary
    {
        IImage CursorImage { get; set; }
        GameTime UpdateGameTime { get; }
    }
}