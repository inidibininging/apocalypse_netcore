using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector
{
    /// <summary>
    /// Builds all enemy mechanics
    /// </summary>
    public class BuildEnemyMechanics : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //var defaultThrustMechanic = new ThrustMechanic();
            //var defaultThrustProxyMechanic = new ThrustProxyImageDataMechanic(defaultThrustMechanic);

            //machine.SharedContext.SingularMechanics.ImageDataMechanics.Add(nameof(ThrustMechanic), defaultThrustProxyMechanic);
            //machine.SharedContext.Messages.Add($"added  {nameof(ThrustMechanic)} in ImageDataMechanics");

            //var randomRotationMechanic = new RandomRotationMechanic();
            //var enemiesMoveRandomlyMechanic = new MoveRandomlyMechanic(defaultThrustMechanic, randomRotationMechanic);
            //machine.SharedContext.SingularMechanics.EnemyMechanics.Add(nameof(MoveRandomlyMechanic), enemiesMoveRandomlyMechanic);
            //machine.SharedContext.Messages.Add($"added  {nameof(MoveRandomlyMechanic)} in EnemyMechanics");
        }
    }
}