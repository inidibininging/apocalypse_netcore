using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Microsoft.Extensions.Logging;
using System;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class ErrorNetworkGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            try
            {
                throw new InvalidOperationException(networkCommandConnectionToHandle.Data);
            }
            catch (Exception ex)
            {
                gameStateContext.Logger.Log(LogLevel.Error, ex.Message + Environment.NewLine + ex.InnerException.Message);
            }

            gameStateContext.ChangeHandlerEasier(gameStateContext.GameStateRegistrar.GetNeworkLayerState((byte)ServerInternalGameStates.Login), networkCommandConnectionToHandle);
        }
    }
}