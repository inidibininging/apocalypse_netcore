using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics
{
    public class UpdateSectorStatusMechanic
        : ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>
    {
        public bool Active { get; set; } = true;

        public IGameSectorsOwner Update(IGameSectorsOwner entity)
        {
            foreach (var sector in
                entity.GameSectorLayerServices.Values.Where(s => s.SharedContext.CurrentStatus == Domain.Server.Model.Interfaces.GameSectorStatus.Running && !s.SharedContext.DataLayer.Players.Any()))
            {
                Console.WriteLine($"Sleep well sector {sector.SharedContext.Tag}");
                sector.SharedContext.CurrentStatus = GameSectorStatus.StandBy;
            }
            foreach (var sector in
                entity.GameSectorLayerServices.Values.Where(s => s.SharedContext.CurrentStatus == Domain.Server.Model.Interfaces.GameSectorStatus.StandBy && s.SharedContext.DataLayer.Players.Any()))
            {
                Console.WriteLine($"Wake up sector {sector.SharedContext.Tag}");
                sector.SharedContext.CurrentStatus = GameSectorStatus.Running;
            }
            return entity;
        }
    }
}
