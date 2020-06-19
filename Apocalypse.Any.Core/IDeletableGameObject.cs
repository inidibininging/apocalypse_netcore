namespace Apocalypse.Any.Core
{
    /// <summary>
    /// This interface says that a game object can be deleted from a dictionary
    /// </summary>
    public interface IDeletableGameObject : IUpdateableLite
    {
        bool MarkedForDeletion { get; }

        void MarkForDeletion();
    }
}