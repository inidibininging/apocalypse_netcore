using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.States.Interfaces
{
    /// <summary>
    /// Makes the connection between the game world and the network layer states
    /// </summary>
    /// <typeparam name="TNetworkLayerIdentifier"></typeparam>
    public interface IGameStateService<TNetworkLayerIdentifier, TWorld>
        : IWorldGameStateDataIOLayerOwner<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        INetworkLayerState<TWorld> GetNeworkLayerState(TNetworkLayerIdentifier identifier);
    }
}