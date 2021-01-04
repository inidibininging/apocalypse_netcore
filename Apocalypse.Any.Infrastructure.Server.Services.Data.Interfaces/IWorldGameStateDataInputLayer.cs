using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IWorldGameStateDataInputLayer<TGameStateData>
    {
        TGameStateData GetGameStateByLoginToken(string loginToken);

        TGameStateData RegisterGameStateData(string loginToken);

        
    }
}