using Apocalypse.Any.Core.Collision;
using System;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface UIElement :
        IVisualGameObject,
        ICollidable,
        IFullPositionHolder
    {
        bool IsVisible { get; set; }

        void OnClick(IGameObject sender, EventArgs args);

        void OnHover(IGameObject sender, EventArgs args);
    }
}