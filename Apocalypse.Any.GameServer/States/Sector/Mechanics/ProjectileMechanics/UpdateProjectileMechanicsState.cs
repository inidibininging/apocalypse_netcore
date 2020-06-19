using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.GameServer.Mechanics.Proxy;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.ProjectileMechanics
{
    public class UpdateProjectileMechanicsState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine.SharedContext.SingularMechanics.ProjectileMechanics.ToList().ForEach(projectileMechanic =>
            {
                machine.SharedContext.DataLayer.Projectiles.ToList().ForEach(projectile => 
                {
                    projectileMechanic.Value.Update(projectile);
                });
            });
        }
    }
}