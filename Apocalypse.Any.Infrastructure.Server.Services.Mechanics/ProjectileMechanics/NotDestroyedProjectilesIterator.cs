using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using System.Collections.Generic;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.ProjectileMechanics
{
    public class DestroyedProjectilesIterator
        : IEnumerableEntityWithImage<Projectile>
    {
        public IEnumerable<Projectile> Update(IEnumerable<Projectile> enumerables) => enumerables.Where(projectile => !projectile.Destroyed);
    }
}