using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface IEnumerableFullPositionHolderMechanic<TEntity>
        : IEnumerableMechanic<TEntity, IFullPositionHolder>
        where TEntity : IFullPositionHolder, new()
    {
    }
}