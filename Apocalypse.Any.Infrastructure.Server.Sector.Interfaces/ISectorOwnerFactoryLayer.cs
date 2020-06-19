using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.Sector.Interfaces
{
    public interface ISectorOwnerFactoryLayers<TSectorOwnerKey, TSectorOwner>
        where TSectorOwner : IGameSectorsOwner
    {
        IDictionary<TSectorOwnerKey, ISingleUpdeatableMechanic<TSectorOwner, IGameSectorsOwner>> SectorMechanics { get; set; }
    }
}