using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Data;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class BuildGameStateDataLayerState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine.SharedContext.IODataLayer = new InMemoryGameStateDataLayer(
                                                        new PlayerSpaceshipGameStateDataFactory(),
                                                        new PlayerSpaceshipFactory());
        }
    }
}