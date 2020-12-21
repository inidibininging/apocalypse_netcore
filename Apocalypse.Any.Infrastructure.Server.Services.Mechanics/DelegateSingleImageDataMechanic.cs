using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class DelegateSingleImageDataMechanic<TEntity>
    : ISingleEntityWithImageMechanic<TEntity>
    where TEntity : IEntityWithImage, new()
    {
        public bool Active { get; set; } = true;
        private Func<TEntity, TEntity> EntityDelegate { get; }
        public DelegateSingleImageDataMechanic(
            Func<TEntity, TEntity> entityDelegate)
        {
            EntityDelegate = entityDelegate ?? throw new ArgumentNullException(nameof(entityDelegate));
        }

        public TEntity Update(TEntity entity)
        {
            return EntityDelegate(entity);
        }
    }
}
