using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    /// <summary>
    /// Bundle of input and output layer as an interface
    /// </summary>
    public interface IWorldGameStateDataIOLayer
        : IWorldGameStateDataInputLayer<GameStateData>,
        IWorldGameStateDataOutputLayer<GameStateData, GameStateUpdateData>
    {
    }
}