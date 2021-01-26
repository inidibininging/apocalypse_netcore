using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class LoginSuccessfulToUpdateGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {

        private INetworkCommandConnectionToGameStateTranslator CurrentNetworkCommandToUpdateGameState { get; set; }

        public LoginSuccessfulToUpdateGameState(INetworkCommandConnectionToGameStateTranslator networkCommandToUpdateGameState)
        {
            if (networkCommandToUpdateGameState == null)
                throw new ArgumentNullException(nameof(networkCommandToUpdateGameState));
            CurrentNetworkCommandToUpdateGameState = networkCommandToUpdateGameState;
        }

        private bool HasValidGameState(NetworkCommandConnection networkCommandConnection)
        {
            if (networkCommandConnection == null)
                return false;
            if (string.IsNullOrWhiteSpace(networkCommandConnection?.CommandArgument))
                return false;
            var typeArgumentAsString = networkCommandConnection.CommandArgument;
            if (string.IsNullOrWhiteSpace(typeArgumentAsString))
                return false;
            return true;
        }

        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            if (!HasValidGameState(networkCommandConnectionToHandle))
                gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Error);

            var typeToProofAgainst = typeof(UserData).FullName;
            if (networkCommandConnectionToHandle.CommandArgument != typeToProofAgainst)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Cannot transition user to update state. Command argument is not of type user data. Type is {networkCommandConnectionToHandle.CommandArgument}");
                var errorHandler = gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Error);
                gameStateContext.ChangeHandlerEasier(gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Error), networkCommandConnectionToHandle);
            }
            else
            {
                var userRoleGateway = gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.UserRoleGateWay);
                gameStateContext.ChangeHandlerEasier(userRoleGateway, networkCommandConnectionToHandle);
            }
            gameStateContext[networkCommandConnectionToHandle.ConnectionId].Handle(gameStateContext, networkCommandConnectionToHandle);
        }
    }
}