using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Newtonsoft.Json;
using System;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class UpdateNetworkGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private INetworkCommandConnectionToGameStateTranslator CurrentNetworkCommandToUpdateGameState { get; set; }
        public ISerializationAdapter SerializationAdapter { get; }

        public UpdateNetworkGameState(INetworkCommandConnectionToGameStateTranslator networkCommandToUpdateGameState, ISerializationAdapter serializationAdapter)
        {
            if (networkCommandToUpdateGameState == null)
                throw new ArgumentNullException(nameof(networkCommandToUpdateGameState));
            CurrentNetworkCommandToUpdateGameState = networkCommandToUpdateGameState;
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }

        private static bool HasValidGameStateData(GameStateData gameStateData)
        {
            if (gameStateData == null ||
                string.IsNullOrWhiteSpace(gameStateData?.LoginToken) ||
                string.IsNullOrWhiteSpace(gameStateData?.Id))
                return false;
            return true;
        }

        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnection)
        {
            if (string.IsNullOrWhiteSpace(networkCommandConnection?.CommandArgument))
                return;
            var typeArgumentAsString = networkCommandConnection.CommandArgument;
            if (string.IsNullOrWhiteSpace(typeArgumentAsString))
                return;

            var gameStateUpdateDataTypeFull = typeof(GameStateUpdateData).FullName;
            if (typeArgumentAsString != gameStateUpdateDataTypeFull) return;
            
            Console.WriteLine($"FULL IN {nameof(UpdateNetworkGameState<TWorld>)} ");
            var clientData = SerializationAdapter.DeserializeObject<GameStateUpdateData>(networkCommandConnection.Data);

            gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.ForwardClientDataToGame(clientData);

            var serverGameState = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetGameStateByLoginToken(clientData.LoginToken);
                
            gameStateContext.ChangeHandlerEasier(gameStateContext[(byte)ServerInternalGameStates.UpdateDelta], networkCommandConnection);
                
            gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient
            (
                NetworkCommandConstants.UpdateCommand,
                serverGameState,
                networkCommandConnection.Connection
            );
        }
    }
}