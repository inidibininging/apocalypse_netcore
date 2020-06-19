using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface ISingleEntityWithImageMechanic<TEntity>
        : ISingleMechanic<TEntity, IEntityWithImage>
        where TEntity : IEntityWithImage
    {
        TEntity Update(TEntity singularEntity);
    }
}