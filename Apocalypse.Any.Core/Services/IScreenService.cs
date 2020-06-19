using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Core.Screen;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace Apocalypse.Any.Core.Services
{
    public interface IScreenService : IUpdateableLite, IVisualGameObject
    {
        bool ChangingScreen { get; }
        ContentManager Content { get; }
        Vector2 Resolution { get; }

        void Initialize<T>() where T : GameScreen, new();

        void ChangeScreen<T>() where T : GameScreen, new();
    }
}