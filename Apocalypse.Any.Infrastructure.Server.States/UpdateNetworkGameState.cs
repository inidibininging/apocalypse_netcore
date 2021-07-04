using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Newtonsoft.Json;
using System;
using Echse.Net.Serialization;
using Microsoft.Extensions.Logging;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class UpdateNetworkGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private INetworkCommandConnectionToGameStateTranslator CurrentNetworkCommandToUpdateGameState { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public UpdateNetworkGameState(INetworkCommandConnectionToGameStateTranslator networkCommandToUpdateGameState, IByteArraySerializationAdapter serializationAdapter)
        {
            CurrentNetworkCommandToUpdateGameState = networkCommandToUpdateGameState ?? throw new ArgumentNullException(nameof(networkCommandToUpdateGameState));
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
            var typeArgumentAsString = networkCommandConnection?.CommandArgument;
            if (string.IsNullOrWhiteSpace(networkCommandConnection?.CommandArgument))
            {
                gameStateContext.Logger.LogError("networkCommandConnection has a CommandArgument with null or empty value");
                return;
            }
                
            // var typeArgumentAsString = networkCommandConnection.CommandArgument;
            //
            // if (string.IsNullOrWhiteSpace(typeArgumentAsString))
            // {
            //     gameStateContext.Logger.LogError("networkCommandConnection Command with null or empty value");
            //     return;
            // }
                

            var gameStateUpdateDataTypeFull = typeof(GameStateUpdateData).FullName;
            if (typeArgumentAsString != gameStateUpdateDataTypeFull) return;
            
            gameStateContext.Logger.Log(LogLevel.Information, $"{nameof(UpdateNetworkGameState<TWorld>)} Deserializing client data as GameStateUpdateData");
            var clientData = SerializationAdapter.DeserializeObject<GameStateUpdateData>(networkCommandConnection.Data);

            gameStateContext.Logger.Log(LogLevel.Information, $"{nameof(UpdateNetworkGameState<TWorld>)} Sending client data to client");
            gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.ForwardClientDataToGame(clientData);

            var serverGameState = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetGameStateByLoginToken(clientData.LoginToken);
                
            gameStateContext.Logger.Log(LogLevel.Information, $"{nameof(UpdateNetworkGameState<TWorld>)} Switching to ServerInternalGameStates.UpdateDelta");
            gameStateContext.ChangeHandlerEasier(gameStateContext[(byte)ServerInternalGameStates.UpdateDelta], networkCommandConnection);

            gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient
            (
                NetworkCommandConstants.UpdateCommand,
                serverGameState,
                Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                0,
                networkCommandConnection.Connection
            );
        }
    }
}