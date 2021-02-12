using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class LoginNetworkGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {

        private INetworkCommandConnectionToGameStateTranslator CurrentNetworkCommandToLoginGameState { get; set; }

        public LoginNetworkGameState(
            INetworkCommandConnectionToGameStateTranslator networkCommandToLoginGameState)
        {
            CurrentNetworkCommandToLoginGameState = networkCommandToLoginGameState ?? throw new ArgumentNullException(nameof(networkCommandToLoginGameState));
        }

        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            try
            {
                //converts user to login token. If user is not registered, it will be... else ..boom
                var gameStateData = CurrentNetworkCommandToLoginGameState.Translate(networkCommandConnectionToHandle);
                
                gameStateContext.Logger.LogInformation( "RegisterGameStateData");
                gameStateData = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.RegisterGameStateData(gameStateData.LoginToken);

                gameStateContext.Logger.LogInformation("SendToClient UpdateCommand register token data");
                gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient(NetworkCommandConstants.UpdateCommand, gameStateData, Lidgren.Network.NetDeliveryMethod.ReliableOrdered, 0, networkCommandConnectionToHandle.Connection);

                gameStateContext.Logger.LogInformation("LoginSuccessful");
                gameStateContext.ChangeHandlerEasier(gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.LoginSuccessful), networkCommandConnectionToHandle);
            }
            catch (System.Exception ex)
            {
                gameStateContext.Logger.LogError(ex.Message);
                gameStateContext.Logger.LogError(ex.InnerException?.Message);
                gameStateContext.ChangeHandlerEasier(gameStateContext[(byte)ServerInternalGameStates.Error], networkCommandConnectionToHandle);
            }
            gameStateContext[networkCommandConnectionToHandle.ConnectionId].Handle(gameStateContext, networkCommandConnectionToHandle);
        }
    }
}