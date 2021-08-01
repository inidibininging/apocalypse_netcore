using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Echse.Net.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Apocalypse.Any.Domain.Server.Model;
using Microsoft.Extensions.Logging;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    /// <summary>
    /// Used for gathering data from the game
    /// </summary>
    public class ReceiveWorkNetworkGameState<TWorld>
        : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private NetworkCommandDataConverterService NetworkCommandDataConverterService { get; }

        public ReceiveWorkNetworkGameState(NetworkCommandDataConverterService networkCommandDataConverterService)
        {
            NetworkCommandDataConverterService = networkCommandDataConverterService ?? throw new ArgumentNullException(nameof(networkCommandDataConverterService));
        }
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            throw new NotImplementedException();
            // //If login command is here. it means that this state is fired first
            // if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.LoginCommand)
            // {
            //     //send an "ACK" for the worker
            //     gameStateContext.Logger.LogInformation($"{nameof(ReceiveWorkNetworkGameState<TWorld>)} ACK on login");
            //     gameStateContext
            //         .CurrentNetOutgoingMessageBusService
            //         .SendToClient(NetworkCommandConstants.ReceiveWorkCommand,
            //             NetworkCommandConstants.OutOfSyncCommand,
            //                       Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
            //                       0,
            //                       networkCommandConnectionToHandle.Connection);
            //     
            //     return;
            // }
            //
            // //If receive command
            // if (networkCommandConnectionToHandle.CommandName != NetworkCommandConstants.ReceiveWorkCommand)
            // {
            //     gameStateContext.Logger.LogError($"{nameof(ReceiveWorkNetworkGameState<TWorld>)} Invalid state found for {networkCommandConnectionToHandle.ConnectionId}. Command name is not ReceiveWorkCommand. Given {networkCommandConnectionToHandle.CommandName} with {networkCommandConnectionToHandle.CommandArgument}");
            //     return;
            // }
            //
            // if (networkCommandConnectionToHandle.CommandArgument != typeof(ReceiveGameStateDataLayerPartRequest).FullName)
            // {
            //     gameStateContext.Logger.LogError($"{nameof(ReceiveWorkNetworkGameState<TWorld>)} Invalid state found for {networkCommandConnectionToHandle.ConnectionId}. No command argument given as {nameof(ReceiveGameStateDataLayerPartRequest)} ");                    
            //     gameStateContext.ChangeHandlerEasier(gameStateContext[(byte)ServerInternalGameStates.Error], networkCommandConnectionToHandle); 
            // }
            //
            // var syncClientRequest = NetworkCommandDataConverterService.ConvertToObject(networkCommandConnectionToHandle) as ReceiveGameStateDataLayerPartRequest;


        }
    }
}
