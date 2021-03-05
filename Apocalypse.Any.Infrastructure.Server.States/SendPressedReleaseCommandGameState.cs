using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    /// <summary>
    /// Emulates that "key was pressed down or released" to the server, until "key up" is sent
    /// </summary>
    /// <typeparam name="TWorld"></typeparam>
    public class SendPressedReleaseCommandGameState<TWorld>  : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private NetworkCommandDataConverterService ConverterService { get; set; }
        private IInputTranslator<int, string> IntToStringCommandTranslator { get; }

        public SendPressedReleaseCommandGameState(
            NetworkCommandDataConverterService converterService, 
            IInputTranslator<int,string> intToStringCommandTranslator)
        {
            ConverterService = converterService;
            IntToStringCommandTranslator = intToStringCommandTranslator;
        }

        public void Handle(INetworkStateContext<TWorld> gameStateContext,
            NetworkCommandConnection networkCommandConnectionToHandle)
        {
            gameStateContext.Logger.LogInformation($" {nameof(SendPressedReleaseCommandGameState<TWorld>)}. Handler");

            //get player login token
            //Route to error
            if (string.IsNullOrWhiteSpace(networkCommandConnectionToHandle.CommandArgument))
            {
                gameStateContext.Logger.LogError($" {nameof(SendPressedReleaseCommandGameState<TWorld>)}. Login token cannot be determined for {networkCommandConnectionToHandle.ConnectionId}");
                gameStateContext.ChangeHandlerEasier(
                    gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte) ServerInternalGameStates.Error),
                    networkCommandConnectionToHandle);
                gameStateContext[networkCommandConnectionToHandle.ConnectionId].Handle(gameStateContext, networkCommandConnectionToHandle);
                return;
            }

            //Jumps in here, if the command login is successful
            //Then an ACK message will be sent
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.LoginCommand)
            {
                 gameStateContext.Logger.LogInformation($"{nameof(SendPressedReleaseCommandGameState<TWorld>)} - Sending ACK for {networkCommandConnectionToHandle.ConnectionId} -> {networkCommandConnectionToHandle.CommandName} {networkCommandConnectionToHandle.CommandArgument}");
                // var user = ConverterService.ConvertToObject(networkCommandConnectionToHandle) as UserData;

                //send an "ACK" to the worker (client)
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

            //Allow to route back to ReceiveGameStateDataLayerPartGameSate if needed
            //ReceiveWorkCommand is needed for acquiring parts of the current game state data layer for example the players or enemies
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.SyncSectorCommand)
            {
                gameStateContext.Logger.LogInformation($" {nameof(SendPressedReleaseCommandGameState<TWorld>)} - Goes now to ReceiveWorkCommand {networkCommandConnectionToHandle.ConnectionId}");
                gameStateContext.ChangeHandlerEasier(
                    gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte) ServerInternalGameStates.ReceiveGameStateDataLayerPart),
                    networkCommandConnectionToHandle);
                gameStateContext[networkCommandConnectionToHandle.ConnectionId].Handle(gameStateContext, networkCommandConnectionToHandle);
                return;
            }

            if (networkCommandConnectionToHandle.CommandName != NetworkCommandConstants.SendPressReleaseCommand)
            {
                gameStateContext.Logger.LogWarning($"{nameof(SendPressedReleaseCommandGameState<TWorld>)} - Invalid state found for {networkCommandConnectionToHandle.ConnectionId}. Expecting SendPressReleaseCommand, was {networkCommandConnectionToHandle.CommandName}");
                return;
            }

            var command = string.Empty;
            var clientInputOnLocalServer = ConverterService.ConvertToObject(networkCommandConnectionToHandle);
            var clientInputConverted = clientInputOnLocalServer as PressReleaseUpdateData ?? new PressReleaseUpdateData() { Command = -1 };
            command = IntToStringCommandTranslator.Translate(clientInputConverted.Command);

            if (string.IsNullOrWhiteSpace(command)) return;

            gameStateContext.Logger.LogInformation($" {nameof(SendPressedReleaseCommandGameState<TWorld>)} SectorKey given: {clientInputConverted.SectorKey}");

            var currentPlayer = gameStateContext
                                        .GameStateRegistrar
                                        .WorldGameStateDataLayer
                                        .GetSector(clientInputConverted.SectorKey)
                                        .DataLayer
                                        .Players
                                        .FirstOrDefault(p => p.LoginToken == clientInputConverted.LoginToken);

            gameStateContext.Logger.LogWarning(currentPlayer?.ToString());

            gameStateContext.Logger.LogInformation($" {nameof(SendPressedReleaseCommandGameState<TWorld>)}. Forwarding Command {command} to sync server {networkCommandConnectionToHandle.ConnectionId}");

            //TODO: forward command to other players
            gameStateContext.CurrentNetOutgoingMessageBusService.Broadcast(NetworkCommandConstants.BroadcastCommand, new string[] { clientInputConverted.LoginToken, command }, NetDeliveryMethod.ReliableOrdered, 0);

            gameStateContext
                .GameStateRegistrar
                .WorldGameStateDataLayer
                .ForwardClientDataToGame(new GameStateUpdateData()
                {
                    LoginToken = clientInputConverted.LoginToken,
                    Commands = new List<string>() { command },
                    Screen = null // if this causes a problem, get screen data from network login from network state    
                });
        }
    }
}