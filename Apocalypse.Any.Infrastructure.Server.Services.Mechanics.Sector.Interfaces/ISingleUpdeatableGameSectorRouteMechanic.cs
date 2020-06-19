using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Domain.Server.Sector.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces
{
    public interface ISingleUpdeatableGameSectorRouteMechanic
        : ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>
    {
        IEnumerable<EntityGameSectorRoute> EntityGameSectorRoutes { get; set; }
    }
}