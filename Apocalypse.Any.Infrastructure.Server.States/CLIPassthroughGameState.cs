using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class CLIPassthroughGameState<TWorld>
        : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            //only "exits" if clear is executed. Clear is triggered by getting the exit command. Command Argument MUST be GameStateUpdateData
            if (networkCommandConnectionToHandle.CommandName == CLINetworkCommandConstants.ExecuteStateCommand)
            {
                networkCommandConnectionToHandle.CommandArgument = typeof(GameStateUpdateData).FullName;
                gameStateContext
                    .GameStateRegistrar
                    .GetNeworkLayerState((byte)ServerInternalGameStates.Update)
                    .Handle(gameStateContext, networkCommandConnectionToHandle);
            }
        }
    }
}