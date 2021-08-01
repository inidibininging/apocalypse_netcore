using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    /// <summary>
    /// Acts as a input layer for acquiring data from the world
    /// </summary>
    /// <typeparam name="TGameStateData"></typeparam>
    public interface IWorldGameStateDataInputLayer<TGameStateData>
    {
        TGameStateData GetGameStateByLoginToken(string loginToken);

        TGameStateData RegisterGameStateData(string loginToken);
       
    }
}