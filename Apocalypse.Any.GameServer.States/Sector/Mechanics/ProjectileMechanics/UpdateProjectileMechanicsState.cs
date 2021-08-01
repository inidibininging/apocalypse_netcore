using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.ProjectileMechanics
{
    /// <summary>
    /// Applies all projectile mechanics to every projectile in the sector
    /// </summary>
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