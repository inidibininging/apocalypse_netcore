using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class RemoveImagesMechanicsState : IState<string, IGameSectorLayerService>
    {
        private IEnumerable<string> Remove { get; } = new List<string>()
        {
            "thrust_6_8"
        };
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine.SharedContext.DataLayer.ImageData = new ConcurrentBag<Domain.Common.Model.Network.ImageData>(
                                                         machine.SharedContext.DataLayer.ImageData.Except(
                                                            machine.SharedContext.DataLayer.ImageData.Where(img => Remove.Contains(img.SelectedFrame))).ToList());
        }
    }
}
