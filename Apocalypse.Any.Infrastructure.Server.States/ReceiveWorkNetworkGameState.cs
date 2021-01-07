using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.DataLayer;
using Apocalypse.Any.GameServer.States.Sector;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    /// <summary>
    /// Used for gathering data from the game
    /// </summary>
    public class ReceiveWorkNetworkGameState<TWorld>
        : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private readonly NetworkCommandDataConverterService NetworkCommandDataConverterService;

        public ReceiveWorkNetworkGameState(NetworkCommandDataConverterService networkCommandDataConverterService)
        {
            NetworkCommandDataConverterService = networkCommandDataConverterService ?? throw new ArgumentNullException(nameof(networkCommandDataConverterService));
        }
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            //If login command is here. it means that this state is fired first
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.LoginCommand)
            {
                //send an "ACK" for the worker
                gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.ReceiveWorkCommand,
                                  true,
                                  Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                                  0,
                                  networkCommandConnectionToHandle.Connection);
                return;
            }

            //If receive command
            if (networkCommandConnectionToHandle.CommandName == NetworkCommandConstants.ReceiveWorkCommand)
            {
                var sectorKey = int.Parse(NetworkCommandDataConverterService.ConvertToObject(networkCommandConnectionToHandle).ToString());
                // if (string.IsNullOrWhiteSpace(sectorKey))
                //     return;
                var sectorLayerService = gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetSector(sectorKey);

                gameStateContext
                    .CurrentNetOutgoingMessageBusService
                    .SendToClient(NetworkCommandConstants.ReceiveWorkCommand,
                                  sectorLayerService.DataLayer as GameStateDataLayer,
                                  Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                                  0,
                                  networkCommandConnectionToHandle.Connection);

                
                return;
            }
        }
    }
}
