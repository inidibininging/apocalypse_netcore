using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Core.Drawing.UI
{
    public interface IChildUIElement
    {
        MovementBehaviour ParentPosition { get; set; }
    }
}