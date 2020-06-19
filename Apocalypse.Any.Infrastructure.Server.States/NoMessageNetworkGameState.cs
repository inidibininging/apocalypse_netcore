using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class NoMessageNetworkGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        public void Handle(INetworkStateContext<TWorld> gameStatecontext, NetworkCommandConnection networkCommandConnection)
        {
            if (networkCommandConnection == null)
                return;
            gameStatecontext.ChangeHandlerEasier(gameStatecontext.GameStateRegistrar.GetNeworkLayerState((byte)ServerInternalGameStates.Login), networkCommandConnection); //where to?
            gameStatecontext[networkCommandConnection.ConnectionId].Handle(gameStatecontext, networkCommandConnection);
        }
    }
}