using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.SectorMechanics
{
    public class SwitchSectorsStatusMechanic
        : ISingleUpdeatableGameSectorRouteMechanic
    {
        public IEnumerable<EntityGameSectorRoute> EntityGameSectorRoutes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public bool Active { get; set; }

        public IGameSectorsOwner Update(IGameSectorsOwner entity)
        {
            if (!Active)
                return entity;

            foreach(var sector in entity.GameSectorLayerServices.Values)
            {
                sector.SharedContext.CurrentStatus = sector.SharedContext.DataLayer.Players.Count > 0 ? GameSectorStatus.Running : GameSectorStatus.StandBy;
            }

            return entity;
        }
    }
}
