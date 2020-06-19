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

        private bool HasValidGameStateData(GameStateData gameStateData)
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

            /*
             * TODO: this doesnt belong here.
             * OR... else?
             * this is a state transition move on the server side, where the client / player receives an update command for the first time
             * this means that the state of the client is changed here to "ok client, you can send me updates now"
            */
            var userDataTypeFull = typeof(UserData).FullName;
            //if (typeArgumentAsString == userDataTypeFull)
            //{
            //    Console.WriteLine("UpdateGameState:Received UserData instead.");
            //    //get login game state through the network translator
            //    //var gameStateData = gameStateContext.NetworkCommandConnectionToGameStateTranslators.GetTranslator(networkCommandConnection);
            //    var gameStateData = CurrentNetworkCommandToUpdateGameState.Translate(networkCommandConnection);

            //    gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient
            //    (
            //        NetworkCommandConstants.UpdateCommand,
            //        gameStateData,
            //        networkCommandConnection.Connection
            //    );
            //}

            var gameStateUpdateDataTypeFull = typeof(GameStateUpdateData).FullName;
            if (typeArgumentAsString == gameStateUpdateDataTypeFull)
            {
                // Console.WriteLine($"{nameof(UpdateGameState)}:Received GameStateUpdateData");
                var clientData = SerializationAdapter.DeserializeObject<GameStateUpdateData>(networkCommandConnection.Data);

                // Console.WriteLine($"{nameof(UpdateGameState)}:Forwarding client data to World");
                gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.ForwardClientDataToGame(clientData);

                // Console.WriteLine($"{nameof(UpdateGameState)}:Get game state by login token");
                var serverGameState = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetGameStateByLoginToken(clientData.LoginToken);
                //Console.WriteLine(serverGameState.Images.Count);
                // Console.WriteLine($"{nameof(UpdateGameState)}:Sending to client");
                gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient
                (
                    NetworkCommandConstants.UpdateCommand,
                    serverGameState,
                    networkCommandConnection.Connection
                );
            }
        }
    }
}