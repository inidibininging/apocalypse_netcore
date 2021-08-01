using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.ItemMechanics
{
    public class NotTakenItemsIterator: IEnumerableCharacterEntityMechanic<Item>
    {
        public IEnumerable<Item> Update(IEnumerable<Item> enumerables) => enumerables.Where(item => !item.InInventory);
    }
}