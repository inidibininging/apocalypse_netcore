using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.EnemyMechanics
{
    public class NotTakenItemsIterator: IEnumerableCharacterEntityMechanic<Item>
    {
        public IEnumerable<Item> Update(IEnumerable<Item> enumerables) => enumerables.Where(item => !item.Taken);
    }
}