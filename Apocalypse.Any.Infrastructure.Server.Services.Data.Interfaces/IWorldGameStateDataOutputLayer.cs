using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    /// <summary>
    /// Acts a middleware for either sending data to the server or to the client
    /// </summary>
    public interface IWorldGameStateDataOutputLayer<TGameStateData, TGameStateUpdateData>
    {
        /// <summary>
        /// Sends update date of a client to the game
        /// </summary>
        /// <param name="updateData"></param>
        /// <returns></returns>
        bool ForwardClientDataToGame(TGameStateUpdateData updateData);

        /// <summary>
        /// Sends actual game state update data to the client
        /// </summary>
        /// <param name="gameStateData"></param>
        /// <returns></returns>
        bool ForwardServerDataToGame(TGameStateData gameStateData);


        /// <summary>
        /// Flushes the game state data if possible, leaving only the player.
        /// </summary>
        /// <param name="loginToken"></param>
        /// <returns></returns>
        //bool FlushGameStateData(string loginToken);
    }
}