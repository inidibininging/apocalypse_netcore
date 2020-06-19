using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface IEnumerableEntityWithImage<TEntity>
        : IEnumerableMechanic<TEntity, IEntityWithImage>
        where TEntity : IEntityWithImage, new()
    {
    }
}