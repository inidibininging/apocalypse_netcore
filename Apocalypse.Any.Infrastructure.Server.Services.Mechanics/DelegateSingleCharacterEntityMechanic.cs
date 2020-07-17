using System;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class DelegateSingleCharacterEntityMechanic<TEntity>
    : ISingleCharacterEntityMechanic<TEntity>
    where TEntity : CharacterEntity, new()
    {
        public bool Active { get; set; } = true;
        private Func<TEntity,TEntity> EntityDelegate { get; }
        public DelegateSingleCharacterEntityMechanic(
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
