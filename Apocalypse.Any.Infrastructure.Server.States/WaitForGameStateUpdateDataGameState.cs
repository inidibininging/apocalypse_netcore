using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Echse.Net.Domain;
using Microsoft.Extensions.Logging;
using System;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class WaitForGameStateUpdateDataGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            var networkCommandConnection = networkCommandConnectionToHandle;

            if (string.IsNullOrWhiteSpace(networkCommandConnection?.CommandArgument))
            {
                gameStateContext.Logger.Log(LogLevel.Warning, $"network command conecction command argument is empty on {nameof(WaitForGameStateUpdateDataGameState<TWorld>)}");
                return;
            }
            var typeArgumentAsString = networkCommandConnection.CommandArgument;
            if (string.IsNullOrWhiteSpace(typeArgumentAsString))
            {

                gameStateContext.Logger.Log(LogLevel.Warning, $"network command conecction command argument is empty on {nameof(WaitForGameStateUpdateDataGameState<TWorld>)}");
                return;
            }

            var userDataTypeFull = typeof(UserData).FullName;
            if (typeArgumentAsString == userDataTypeFull)
            {
                gameStateContext.Logger.Log(LogLevel.Debug, nameof(WaitForGameStateUpdateDataGameState<TWorld>));
                //get login game state through the network translator

                //first: check if client ALREADY has token.

                //if so... there is a need here for a converter from user to login token
                //gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient
                //(
                //    NetworkCommandConstants.UpdateCommand,
                //    gameStateData,
                //    networkCommandConnection.Connection
                //);
            }
        }
    }
}