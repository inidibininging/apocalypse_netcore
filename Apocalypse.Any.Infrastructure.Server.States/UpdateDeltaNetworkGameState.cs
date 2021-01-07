using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    /// <summary>
    /// This class sends first GameStateData packages to the client.
    /// If there is no problem with the client, this class will send DeltaGameState packages, which are only the diff between the last successful "probably" received package 
    /// and the 
    /// </summary>
    /// <typeparam name="TWorld"></typeparam>
    public class UpdateDeltaNetworkGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private INetworkCommandConnectionToGameStateTranslator CurrentNetworkCommandToUpdateGameState { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public UpdateDeltaNetworkGameState(
            INetworkCommandConnectionToGameStateTranslator networkCommandToUpdateGameState,
            IByteArraySerializationAdapter serializationAdapter,
            IDeltaGameStateDataService deltaGameStateDataService)
        {
            CurrentNetworkCommandToUpdateGameState = networkCommandToUpdateGameState ?? throw new ArgumentNullException(nameof(networkCommandToUpdateGameState)); ;
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
            DeltaGameStateDataService = deltaGameStateDataService ?? throw new ArgumentNullException(nameof(deltaGameStateDataService));
        }

        private static bool HasValidGameStateData(GameStateData gameStateData)
        {
            if (gameStateData == null ||
                string.IsNullOrWhiteSpace(gameStateData?.LoginToken) ||
                string.IsNullOrWhiteSpace(gameStateData?.Id))
                return false;
            return true;
        }
        private IDeltaGameStateDataService DeltaGameStateDataService { get; }
        private GameStateData LastGameStateData { get; set; }

        public TimeSpan LastDiff { get; set; }
        public const int DiffMiliseconds = 1000; 
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnection)
        {
            if (string.IsNullOrWhiteSpace(networkCommandConnection?.CommandArgument))
                return;
            var typeArgumentAsString = networkCommandConnection.CommandArgument;
            if (string.IsNullOrWhiteSpace(typeArgumentAsString))
                return;

            var gameStateUpdateDataTypeFull = typeof(GameStateUpdateData).FullName;
            if (typeArgumentAsString == gameStateUpdateDataTypeFull)
            {
                var clientData = SerializationAdapter.DeserializeObject<GameStateUpdateData>(networkCommandConnection.Data);

                var serverGameState = gameStateContext
                                        .GameStateRegistrar
                                        .WorldGameStateDataLayer
                                        .GetGameStateByLoginToken(clientData.LoginToken);                             
                

                if (LastGameStateData == null || networkCommandConnection.CommandName == NetworkCommandConstants.UpdateCommand)
                {
                    UpdateLastGameStateData(serverGameState);

                    gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient
                    (
                        NetworkCommandConstants.UpdateCommand,
                        serverGameState,
                        Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                        0,
                        networkCommandConnection.Connection
                    );

                }
                else
                {
                    var a = DateTime.Now;
                    var deltaGameState = DeltaGameStateDataService.GetDelta(LastGameStateData, serverGameState);
                    var sentMessage = gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient
                    (
                        NetworkCommandConstants.UpdateCommandDelta,
                        deltaGameState,
                        Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                        0,
                        networkCommandConnection.Connection
                    );
                    var b = DateTime.Now - a;
                    LastDiff += b;
                    if(sentMessage == Lidgren.Network.NetSendResult.FailedNotConnected || 
                       sentMessage == Lidgren.Network.NetSendResult.Dropped ||
                       LastDiff.TotalMilliseconds > DiffMiliseconds)
                    {
                        LastGameStateData = null; //triggers full update again
                        LastDiff = TimeSpan.Zero;
                    }
                    else
                    {
                        UpdateLastGameStateData(serverGameState);
                    }
                }
                gameStateContext
                    .GameStateRegistrar
                    .WorldGameStateDataLayer
                    .ForwardClientDataToGame(clientData);
            }
        }

        private void UpdateLastGameStateData(GameStateData serverGameState)
        {
            using (var stream = new MemoryStream())
            {
                var serializer = new XmlSerializer(typeof(GameStateData));
                serializer.Serialize(stream, serverGameState);
                stream.Position = 0;
                LastGameStateData = (GameStateData)serializer.Deserialize(stream);
            }
        }
    }
}
