using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class UpdateAllPluralEnemyMechanicsState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //TODO: use a logger here
            machine.SharedContext.PluralMechanics.EnemyMechanics.Values.ToList().ForEach(pluralMechanic =>
            {
                pluralMechanic.Update(machine.SharedContext.DataLayer.Enemies);
            });
        }
    }
}