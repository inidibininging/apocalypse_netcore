using System.Collections.Generic;

namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface IEnumerableMechanic<TEntity, TBaseEntity>
        where TEntity : TBaseEntity, new()
    {
        IEnumerable<TEntity> Update(IEnumerable<TEntity> enumerables);
    }
}