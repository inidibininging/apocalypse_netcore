using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Factories
{
    public interface IItemFactory
    {
        Item Create(IGameSectorBoundaries sectorBoundaries);
    }
}