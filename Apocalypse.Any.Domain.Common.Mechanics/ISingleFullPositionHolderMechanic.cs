using Apocalypse.Any.Core.Drawing;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface ISingleFullPositionHolderMechanic<TEntity>
        : ISingleMechanic<TEntity, IFullPositionHolder>
        where TEntity : IFullPositionHolder
    {
        TEntity Update(TEntity entity);
    }
}