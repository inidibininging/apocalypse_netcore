namespace Apocalypse.Any.Domain.Common.Mechanics
{
    public interface ISingleUpdeatableMechanic<TEntity, TBaseEntity> : ISingleMechanic<TEntity, TBaseEntity>
        where TEntity : TBaseEntity
    {
        TEntity Update(TEntity entity);
    }
}