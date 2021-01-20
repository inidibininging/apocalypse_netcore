using System.Collections.Generic;
using Apocalypse.Any.Core.Input.Translator;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Microsoft.Extensions.Logging;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    /// <summary>
    /// Emulates that "key was pressed down" to the server, until "key up" is sent
    /// </summary>
    /// <typeparam name="TWorld"></typeparam>
    public class CommandDownGameState<TWorld>  : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        public void Handle(INetworkStateContext<TWorld> gameStateContext,
            NetworkCommandConnection networkCommandConnectionToHandle)
        {
            //get player login token
            if (string.IsNullOrWhiteSpace(networkCommandConnectionToHandle.CommandArgument))
            {
                gameStateContext.Logger.LogError($" {nameof(CommandDownGameState<TWorld>)}. Login token cannot be determined for {networkCommandConnectionToHandle.ConnectionId}");
                gameStateContext.ChangeHandlerEasier(
                    gameStateContext.GameStateRegistrar.GetNeworkLayerState((byte) ServerInternalGameStates.Error),
                    networkCommandConnectionToHandle);
                gameStateContext[networkCommandConnectionToHandle.ConnectionId].Handle(gameStateContext, networkCommandConnectionToHandle);
                return;
            }

            string command = "";
            switch (networkCommandConnectionToHandle.CommandName)
            {
                case 0:
                    command = DefaultKeys.Up + DefaultKeys.Press;
                    break;
                case 1:
                    command = DefaultKeys.Down + DefaultKeys.Press;
                    break;
                case 2:
                    command = DefaultKeys.Left + DefaultKeys.Press;
                    break;
                case 3:
                    command = DefaultKeys.Right + DefaultKeys.Press;
                    break;
                case 4:
                    command = DefaultKeys.Shoot + DefaultKeys.Press;
                    break;
                case 5:
                    command = DefaultKeys.Boost + DefaultKeys.Press;
                    break;
                case 6:
                    command = DefaultKeys.AltShoot + DefaultKeys.Press;
                    break;
                case 7:
                    command = DefaultKeys.Up + DefaultKeys.Release;
                    break;
                case 8:
                    command = DefaultKeys.Down + DefaultKeys.Release;
                    break;
                case 9:
                    command = DefaultKeys.Left + DefaultKeys.Release;
                    break;
                case 10:
                    command = DefaultKeys.Right + DefaultKeys.Release;
                    break;
                case 11:
                    command = DefaultKeys.Shoot + DefaultKeys.Release;
                    break;
                case 12:
                    command = DefaultKeys.Boost + DefaultKeys.Release;
                    break;
                case 13:
                    command = DefaultKeys.AltShoot + DefaultKeys.Release;
                    break;
            }
            
            gameStateContext
                .GameStateRegistrar
                .WorldGameStateDataLayer
                .ForwardClientDataToGame(new GameStateUpdateData()
                {
                    LoginToken = networkCommandConnectionToHandle.CommandArgument,
                    Commands = new List<string>() { command },
                    Screen = null // if this causes a problem, get screen data from network login from network state    
                });

        }
    }
}