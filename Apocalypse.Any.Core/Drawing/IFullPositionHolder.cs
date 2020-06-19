namespace Apocalypse.Any.Core.Drawing
{
    /// <summary>
    /// This class says the a game object has access to position data and rotation data
    /// </summary>
    public interface IFullPositionHolder :
        IMovableGameObject,
        IRotatableGameObject
    {
    }
}