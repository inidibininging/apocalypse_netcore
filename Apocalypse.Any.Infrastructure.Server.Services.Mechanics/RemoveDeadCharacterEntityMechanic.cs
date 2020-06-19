using System;
using System.Linq;
using System.Collections.Generic;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics
{
    public class RemoveDeadCharacterEntityMechanic<TCharacterEntity>
    : IEnumerableCharacterEntityMechanic<TCharacterEntity>
    where TCharacterEntity : CharacterEntity, new ()
    {
        public IEnumerable<TCharacterEntity> Update(IEnumerable<TCharacterEntity> enumerables) 
        => enumerables.Where(characterEntity => characterEntity.Stats.Health > characterEntity.Stats.GetMinAttributeValue());
    }
}
