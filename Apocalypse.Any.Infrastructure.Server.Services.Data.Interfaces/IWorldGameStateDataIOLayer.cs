using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IWorldGameStateDataIOLayer
        : IWorldGameStateDataInputLayer<GameStateData>,
        IWorldGameStateDataOutputLayer<GameStateData, GameStateUpdateData>
    {
    }
}