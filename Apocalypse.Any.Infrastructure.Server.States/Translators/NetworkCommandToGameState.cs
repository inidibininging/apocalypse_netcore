using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using System;
using Apocalypse.Any.Domain.Common.Network;

namespace Apocalypse.Any.Infrastructure.Server.States.Translators
{
    public class NetworkCommandToGameState : INetworkCommandConnectionToGameStateTranslator
    {
        private IWorldGameStateDataInputLayer<GameStateData> GameStatesDataLayer { get; set; }

        public NetworkCommandToGameState(IWorldGameStateDataInputLayer<GameStateData> gameStateDataLayer)
        {
            if (gameStateDataLayer == null)
                throw new ArgumentNullException(nameof(gameStateDataLayer));
            GameStatesDataLayer = gameStateDataLayer;
        }

        private bool HasValidGameStateData(GameStateData gameStateData)
        {
            if (gameStateData == null ||
                string.IsNullOrWhiteSpace(gameStateData?.LoginToken))
                return false;
            return true;
        }

        public GameStateData Translate(NetworkCommandConnection input)
        {
            if (input == null)
                return null;
            if (input.CommandName != NetworkCommandConstants.GetGameStateByLoginToken)
                return null;
            if (string.IsNullOrWhiteSpace(input.Data))
                throw new InvalidOperationException("No valid token sent");
            var loginToken = input.Data;
            return GameStatesDataLayer.GetGameStateByLoginToken(loginToken);
        }
    }
}