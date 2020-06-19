using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.ProjectileMechanics
{
    public class RemoveDestroyedProjectilesState : IState<string, IGameSectorLayerService>
    {
        private IEnumerableEntityWithImage<Projectile> NotDestroyedProjectilesProvider { get; set; }

        public RemoveDestroyedProjectilesState(IEnumerableEntityWithImage<Projectile> notDestroyedProjectilesProvider)
        {
            NotDestroyedProjectilesProvider = notDestroyedProjectilesProvider ?? throw new ArgumentNullException(nameof(notDestroyedProjectilesProvider));
        }

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine.SharedContext.DataLayer.Projectiles = new ConcurrentBag<Projectile>(NotDestroyedProjectilesProvider.Update(machine.SharedContext.DataLayer.Projectiles));
        }
    }
}