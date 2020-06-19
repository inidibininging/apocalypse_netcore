using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Lidgren.Network;
using Microsoft.Extensions.Logging;

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

        void Update();

        void ChangeHandlerEasier(INetworkLayerState<TWorld> gameState, NetworkCommandConnection networkCommandConnection);
    }
}