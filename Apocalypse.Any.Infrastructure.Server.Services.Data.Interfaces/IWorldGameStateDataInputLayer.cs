using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IWorldGameStateDataInputLayer
    {
        GameStateData GetGameStateByLoginToken(string loginToken);

        GameStateData RegisterGameStateData(string loginToken);
    }
}