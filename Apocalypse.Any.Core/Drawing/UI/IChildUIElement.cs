
using Apocalypse.Any.Core.Behaviour;
using Microsoft.Xna.Framework;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IChildUIElement
    {
        MovementBehaviour ParentPosition { get; set; }
        Vector2 ParentScale { get; set; }
        float ParentWidth { get; set; }
        float ParentHeight { get; set; }
        
    }
}