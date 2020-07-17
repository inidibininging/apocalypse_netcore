using System;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class DelegateFullPositionHolderMechanic<TEntity>
    : ISingleFullPositionHolderMechanic<TEntity>
    where TEntity : IFullPositionHolder
    {
        public bool Active { get; set; } = true;
        private Func<TEntity,TEntity> EntityDelegate { get; }
        public DelegateFullPositionHolderMechanic(
            Func<TEntity,TEntity> entityDelegate)
        {
            EntityDelegate = entityDelegate ?? throw new ArgumentNullException(nameof(entityDelegate));
        }

        public TEntity Update(TEntity entity)
        {
            return EntityDelegate(entity);
        }
    }
}
