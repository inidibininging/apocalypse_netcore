using Apocalypse.Any.Domain.Common.Mechanics;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Interfaces
{
    public interface ISingleGameSectorOwnerMechanic<TWhoIsMeOwner>
        : ISingleUpdeatableMechanic<TWhoIsMeOwner, TWhoIsMeOwner>
    {
    }
}