using System;
using System.Collections.Generic;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using States.Core.Infrastructure.Services;

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

        public SendPressedReleaseCommandGameState(NetworkCommandDataConverterService converterService, IInputTranslator<int,string> intToStringCommandTranslator)
        {
            ConverterService = converterService;
            IntToStringCommandTranslator = intToStringCommandTranslator;
        }

        public void Handle(INetworkStateContext<TWorld> gameStateContext,
            NetworkCommandConnection networkCommandConnectionToHandle)
        {

            //get player login token
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
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.LoginCommand)
            {
                gameStateContext.Logger.LogInformation($" {nameof(SendPressedReleaseCommandGameState<TWorld>)}. Sending ACK for {networkCommandConnectionToHandle.ConnectionId}");
                //send an "ACK" for the worker (client)
                gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.SendPressReleaseCommand, 
                        true, 
                        NetDeliveryMethod.ReliableOrdered, 
                        0,
                        networkCommandConnectionToHandle.Connection
                    );
                return;
            }
            
            if (networkCommandConnectionToHandle.CommandName != NetworkCommandConstants.SendPressReleaseCommand) return;
            
            var command = string.Empty;
            var clientInput = ConverterService.ConvertToObject(networkCommandConnectionToHandle);
            var clientInputConverted = clientInput as PressReleaseUpdateData ?? new PressReleaseUpdateData() { Command = -1 };
            command = IntToStringCommandTranslator.Translate(clientInputConverted.Command);

            if (string.IsNullOrWhiteSpace(command)) return;
            
            gameStateContext.Logger.LogInformation($" {nameof(SendPressedReleaseCommandGameState<TWorld>)}. Forwarding Command {command} to Server {networkCommandConnectionToHandle.ConnectionId}");
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