﻿using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Factories;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class BuildIODataLayerState : IState<string, IGameSectorLayerService>
    {
        //private static IWorldGameStateDataIOLayer worldGameStateDataIOLayer;
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine.SharedContext.IODataLayer = new InMemoryGameStateDataLayer(
                                                        new PlayerSpaceshipGameStateDataFactory(),
                                                        new PlayerSpaceshipFactory());            
        }
    }
}