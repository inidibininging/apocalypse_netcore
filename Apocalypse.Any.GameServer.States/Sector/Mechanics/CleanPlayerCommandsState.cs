using System;
using System.Collections.Generic;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class CleanPlayerCommandsState : IState<string, IGameSectorLayerService>
    {
        private Dictionary<string, List<string>> KeyDownUp { get; set; } = new();

        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            foreach (var player in machine.SharedContext.DataLayer.Players)
            {
                var playerGameStateData = machine.SharedContext.IODataLayer.GetGameStateByLoginToken(player.LoginToken);

                //clear commands
                playerGameStateData.Commands.Clear();
            }
        }
    }
}
