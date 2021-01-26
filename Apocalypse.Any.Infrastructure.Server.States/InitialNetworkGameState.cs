using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using System;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class InitialNetworkGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private INetworkCommandConnectionToGameStateTranslator CurrentNetworkCommandToInitialGameState { get; set; }

        public InitialNetworkGameState(
            INetworkCommandConnectionToGameStateTranslator networkCommandToInitialGameState)
        {
            CurrentNetworkCommandToInitialGameState = networkCommandToInitialGameState ?? throw new ArgumentNullException(nameof(networkCommandToInitialGameState));
        }

        private bool HasValidGameStateData(GameStateData gameStateData)
        {
            return gameStateData != null && !string.IsNullOrWhiteSpace(gameStateData?.LoginToken);
        }

        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnection)
        {
            if (networkCommandConnection == null)
                return;
            try
            {
                var gameStateData = CurrentNetworkCommandToInitialGameState.Translate(networkCommandConnection);
                if (!HasValidGameStateData(gameStateData))
                {
                    gameStateContext.ChangeHandlerEasier(gameStateContext[(byte)ServerInternalGameStates.Login], networkCommandConnection);
                }
            }
            catch (Exception)
            {
                gameStateContext.ChangeHandlerEasier(gameStateContext[(byte)ServerInternalGameStates.Error], networkCommandConnection);
            }
            gameStateContext[networkCommandConnection.ConnectionId].Handle(gameStateContext, networkCommandConnection);
        }
    }
}