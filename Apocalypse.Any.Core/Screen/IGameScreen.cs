using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Core.Screen
{
    public interface IGameScreen : IVisualGameObject, IGameObjectDictionary
    {
        IImage CursorImage { get; set; }
    }
}