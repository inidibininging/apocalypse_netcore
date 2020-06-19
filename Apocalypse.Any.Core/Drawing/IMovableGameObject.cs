using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Core.Drawing
{
    public interface IMovableGameObject
    {
        MovementBehaviour Position { get; set; }
    }
}