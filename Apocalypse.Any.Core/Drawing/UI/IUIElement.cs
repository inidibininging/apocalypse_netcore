using Apocalypse.Any.Core.Collision;
using System;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IUIElement :
        IVisualGameObject,
        ICollidable,
        IFullPositionHolder
    {

        bool IsVisible { get; set; }
        
    }
}