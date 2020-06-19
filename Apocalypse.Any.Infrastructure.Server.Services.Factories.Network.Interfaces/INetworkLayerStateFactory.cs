using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.Services.Factories.Network.Interfaces
{
    /// <summary>
    /// Creates a network layer state
    /// </summary>
    internal interface INetworkLayerStateFactoryAdapter<TWorld> : IGenericTypeFactory<INetworkLayerState<TWorld>>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
    }
}