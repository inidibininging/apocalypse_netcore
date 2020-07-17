using System;
using Apocalypse.Any.Domain.Common.Mechanics;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class DelegateMechanic<TEntity, TBaseEntity>
        : ISingleMechanic<TEntity, TBaseEntity>
        where TEntity : TBaseEntity
    {
        public bool Active { get; set; } = true;
        private Func<TEntity,TEntity> EntityDelegate { get; }
        public DelegateMechanic(Func<TEntity, TEntity> entityDelegate) => EntityDelegate = entityDelegate ?? throw new ArgumentNullException(nameof(entityDelegate));
        public TEntity Update(TEntity singularEntity) => EntityDelegate(singularEntity);
    }
}
