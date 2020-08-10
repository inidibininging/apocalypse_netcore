using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Common.Services.Data.Interfaces
{
    public interface IDeltaGameStateDataService
    {
        DeltaGameStateData GetDelta(GameStateData gameStateDataBefore, GameStateData gameStateDataAfter);
        GameStateData UpdateGameStateData(GameStateData gameStateData, DeltaGameStateData deltaGameStateData);
    }
}