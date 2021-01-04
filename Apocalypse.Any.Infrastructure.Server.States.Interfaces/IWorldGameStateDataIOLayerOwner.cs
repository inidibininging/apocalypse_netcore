using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.States.Interfaces
{
    public interface IWorldGameStateDataIOLayerOwner<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        TWorld WorldGameStateDataLayer { get; }
    }
}