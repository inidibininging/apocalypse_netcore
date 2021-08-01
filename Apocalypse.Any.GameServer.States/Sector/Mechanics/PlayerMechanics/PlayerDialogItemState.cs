using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics.PlayerMechanics
{
    public class PlayerDialogItemState : IState<string, IGameSectorLayerService>
    {
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            var playerEvents = machine
                                    .SharedContext
                                    .DataLayer
                                    .Layers
                                    .Where(layer => layer.GetValidTypes().Any(t => t == typeof(PlayerDialogEvent<string, Item>)))
                                    .SelectMany(layer => layer.DataAsEnumerable<PlayerDialogEvent<string, Item>>());
        }
    }
}
