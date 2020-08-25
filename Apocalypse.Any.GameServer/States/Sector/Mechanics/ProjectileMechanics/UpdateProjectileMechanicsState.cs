using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.GameServer.Mechanics.Proxy;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Microsoft.Xna.Framework;
using States.Core.Infrastructure.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.ProjectileMechanics
{
    public class UpdateProjectileMechanicsState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {

            machine.SharedContext.SingularMechanics.ProjectileMechanics.ToList().ForEach(projectileMechanic =>
            {
                var projectilesUpdate = Parallel.ForEach(machine.SharedContext.DataLayer.Projectiles, (projectile) => 
                {
                    projectileMechanic.Value.Update(projectile);
                });
                while (!projectilesUpdate.IsCompleted)
                {
                    //Console.WriteLine(".");
                }

            });
        }
    }
}