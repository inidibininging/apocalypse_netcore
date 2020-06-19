using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System.Linq;

namespace Apocalypse.Any.GameServer.States.Sector
{
    public class UpdateAllSingularEnemyMechanicsState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {

            //TODO: use a logger here
            machine
                .SharedContext
                .DataLayer
                .ImageData
                .ToList()
                .ForEach(imgData =>
            {
                machine
                .SharedContext
                .SingularMechanics
                .ImageDataMechanics
                .ToList()
                .ForEach(imgDataMechanics => imgDataMechanics.Value.Update(imgData));
            });

            machine
                .SharedContext
                .DataLayer
                .Enemies
                .ToList()
                .ForEach(enemy =>
            {
                machine
                .SharedContext
                .SingularMechanics
                .EnemyMechanics
                .ToList()
                .ForEach(enemyMechanic => enemyMechanic.Value.Update(enemy));
            });

            machine
                .SharedContext
                .DataLayer
                .Players
                .ToList()
                .ForEach(player =>
            {
                machine
                .SharedContext
                .SingularMechanics
                .PlayerMechanics
                .ToList()
                .ForEach(playerMechanic => playerMechanic.Value.Update(player));
            });

        }
    }
}