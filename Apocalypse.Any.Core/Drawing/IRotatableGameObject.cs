using Apocalypse.Any.Core.Behaviour;

namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// This interface says that a game object can be rotated
    /// </summary>
    public interface IRotatableGameObject
    {
        RotationBehaviour Rotation { get; set; }
    }
}