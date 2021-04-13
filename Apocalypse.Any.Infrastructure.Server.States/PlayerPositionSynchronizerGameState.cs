using System;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.Logging;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    /// <summary>
    /// SyncServer: Checks wether the players coordinates in the local server and sync server are 'roughly equal'
    /// This State always sets the default Handler back to SendPressRelease if anything out of the normal scope happens
    /// </summary>
    /// <typeparam name="TWorld"></typeparam>
    public class PlayerPositionSynchronizerGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private NetworkCommandDataConverterService ConverterService { get; }

        public PlayerPositionSynchronizerGameState(NetworkCommandDataConverterService converterService)
        {
            ConverterService = converterService;
        }

        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            // This state should only handle the check of players position by its login token. 
            // This state routes back to sendpressrelease if the command name entered is not PlayerPositionSync
            // Otherwise it will cause an unexpected behaviour
            if(networkCommandConnectionToHandle.CommandName != NetworkCommandConstants.PlayerPositionSync){
                gameStateContext.Logger.LogError($"{nameof(PlayerPositionSynchronizerGameState<TWorld>)} Command name is not PlayerPositionSync. Given: {networkCommandConnectionToHandle.CommandName}. Going back to SendPressedRelease");
                var back = gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.SendPressedRelease);
                gameStateContext.ChangeHandlerEasier(back, networkCommandConnectionToHandle);
                return;
            }

            var playerPositionUpdate = ConverterService.ConvertToObject(networkCommandConnectionToHandle) as PlayerPositionUpdateData;

            // If this state is called by a sync server, forward the other players data to the game data layer
            // This means that the data comes from a local server
            // TODO: Forwarding someone players pos in the game can be exploited for now because there is no check for the senders data
            if(gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.Source == UserDataRoleSource.SyncServer) {
                var playersSector = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector(playerPositionUpdate.SectorKey);
                gameStateContext.Logger.LogInformation($"POSITION DATA = SECTOR:{playerPositionUpdate.SectorKey}, LOGINTOKEN: {playerPositionUpdate.LoginToken} POS:{playerPositionUpdate.X},{playerPositionUpdate.Y},{playerPositionUpdate.R}");
                // If the sector was not found, the sync server and local server have not the same data. Maybe a hack / mod ?       
                if(playersSector == null)
                {
                    gameStateContext.Logger.LogWarning($"{nameof(PlayerPositionSynchronizerGameState<TWorld>)} SOMETHING IS WRONG. KICKING PLAYER {networkCommandConnectionToHandle.ConnectionId}");
                    //Le kick ...
                    gameStateContext.Logger.LogError($"{nameof(PlayerPositionSynchronizerGameState<TWorld>)} Command name is PlayerPositionSync but the sector requested was not found on the local server. WHAT ARE YOU DOING?");
                    var back = gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Error);
                    gameStateContext.ChangeHandlerEasier(back, networkCommandConnectionToHandle);
                    return;
                }

                // If the player is not in the sector, it is because the player is either in a private instance e.g a shop 
                // or because the sync server and local server are not in sync e.g player joined the game recently
                var player = playersSector.DataLayer.Players.FirstOrDefault(p => playerPositionUpdate.LoginToken == p.LoginToken);
                if(player == null) {
                    gameStateContext.Logger.LogWarning($"{nameof(PlayerPositionSynchronizerGameState<TWorld>)} PLAYER IS NULL {networkCommandConnectionToHandle.ConnectionId}");
                    gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.SendPressReleaseCommand,
                        NetworkCommandConstants.OutOfSyncCommand, //Tells the sync client that is is out of sync. Next logical step should be ReceiveGameStateDataLayerPartGameSate
                        NetDeliveryMethod.ReliableOrdered,
                        0,
                        networkCommandConnectionToHandle.Connection
                    );
                    return;
                }

                // Check if the difference of this sync server input and the local server input
                const float errorMargin = 1;
                var xErrorMargin = MathF.Abs(player.CurrentImage.Position.X - playerPositionUpdate.X);
                var yErrorMargin = MathF.Abs(player.CurrentImage.Position.Y - playerPositionUpdate.Y);
                var rErrorMargin = MathF.Abs(player.CurrentImage.Rotation.Rotation - playerPositionUpdate.R);
                gameStateContext.Logger.LogInformation($"Error margin: X: {xErrorMargin} Y {yErrorMargin} R {rErrorMargin}");

                if(xErrorMargin > errorMargin || yErrorMargin > errorMargin || rErrorMargin > errorMargin) {
                    gameStateContext.Logger.LogWarning("OH OH ... error margin reached");
                    gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.SendPressReleaseCommand,
                        NetworkCommandConstants.OutOfSyncCommand, //Tells the sync client that is is out of sync. Next logical step should be ReceiveGameStateDataLayerPartGameSate
                        NetDeliveryMethod.ReliableOrdered,
                        0,
                        networkCommandConnectionToHandle.Connection
                    );
                    return;
                }
                else {
                    // pass local client data from local server to sync server. Is this REALLY a good idea?
                    player.CurrentImage.Position.X = playerPositionUpdate.X;
                    player.CurrentImage.Position.Y = playerPositionUpdate.Y;
                    player.CurrentImage.Rotation.Rotation = playerPositionUpdate.R;
                }
                return;
            }


            if(playerPositionUpdate == null) {
                gameStateContext.Logger.LogError($"{nameof(PlayerPositionSynchronizerGameState<TWorld>)} Cannot convert players position. Given Command: {networkCommandConnectionToHandle.CommandName} CommandArgument: {networkCommandConnectionToHandle.CommandArgument}. Going back to SendPressedRelease");
                var back = gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.SendPressedRelease);
                gameStateContext.ChangeHandlerEasier(back, networkCommandConnectionToHandle);
                return;
            }


        }
    }
}

