using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.States.Interfaces
{
    public interface INetworkStateContext<TWorld>
       where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        //IGameState CurrentHandler { get; set; }
        //TODO: maybe change this to a Span?
        INetworkLayerState<TWorld> this[long clientIdentifier]
        {
            get;
        }

        //INetworkCommandConnectionToGameStateTranslatorService NetworkCommandConnectionToGameStateTranslators { get; }

        IGameStateService<byte, TWorld> GameStateRegistrar { get; }

        NetOutgoingMessageBusService<NetServer> CurrentNetOutgoingMessageBusService { get; }

        ILogger<byte> Logger { get;  }

        void ForwardIncomingMessagesToHandlers(List<NetIncomingMessage> messageChunk);

        void ChangeHandlerEasier(INetworkLayerState<TWorld> gameState, NetworkCommandConnection networkCommandConnection);
    }
}