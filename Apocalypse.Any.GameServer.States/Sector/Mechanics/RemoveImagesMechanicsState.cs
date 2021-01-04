using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apocalypse.Any.Constants;

namespace Apocalypse.Any.GameServer.States.Sector.Mechanics
{
    public class RemoveImagesMechanicsState : IState<string, IGameSectorLayerService>
    {
        private IEnumerable<(int frame, int x, int y)> Remove { get; } = new List<(int frame, int x, int y)>()
        {
            (ImagePaths.ThrustFrame, 6, 8)
        };
        public void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            machine.SharedContext.DataLayer.ImageData = new ConcurrentBag<ImageData>(
                                                         machine.SharedContext.DataLayer.ImageData.Except(
                                                         machine.SharedContext.DataLayer.ImageData.Where(img => Remove.Contains(img.SelectedFrame))).ToList());
        }
    }
}
