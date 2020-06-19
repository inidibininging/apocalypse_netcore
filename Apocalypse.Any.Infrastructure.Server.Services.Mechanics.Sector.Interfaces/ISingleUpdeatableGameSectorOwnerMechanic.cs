using Apocalypse.Any.Domain.Common.Mechanics;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Sector.Interfaces
{
    public interface ISingleUpdeatableGameSectorOwnerMechanic
        : ISingleUpdeatableMechanic<IGameSectorsOwner, IGameSectorsOwner>
    {
    }
}