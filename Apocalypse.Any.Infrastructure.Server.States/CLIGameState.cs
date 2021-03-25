using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using System;
using System.Collections.Concurrent;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class CLIGameState<TWorld> 
        : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        //consider this a fix for now THAT needs to be cleaned in the future. What if a user doesnt have the rights anymore????
        private ConcurrentDictionary<long, UserDataWithLoginToken> CachedAuthorizedUsers { get; set; } = new ConcurrentDictionary<long, UserDataWithLoginToken>();

        private IInputTranslator<NetworkCommandConnection, UserDataWithLoginToken> CurrentNetworkCommandToFullUserDataTranslator { get; set; }

        public CLIGameState(IInputTranslator<NetworkCommandConnection, UserDataWithLoginToken> networkCommandToFullUserDataTranslator)
        {
            if (networkCommandToFullUserDataTranslator == null)
                throw new ArgumentNullException(nameof(networkCommandToFullUserDataTranslator));
            CurrentNetworkCommandToFullUserDataTranslator = networkCommandToFullUserDataTranslator;
        }

        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            //Passes through if network command id was already registered as a CLI user.
            UserDataWithLoginToken userData = null;
            if (!CachedAuthorizedUsers.ContainsKey(networkCommandConnectionToHandle.ConnectionId))
            {
                userData = CurrentNetworkCommandToFullUserDataTranslator.Translate(networkCommandConnectionToHandle);

                //check permissions
                if (userData.Roles == null || !userData.Roles[UserDataRoleSource.SyncServer].HasFlag(UserDataRole.CanSendRemoteStateCommands))
                {
                    gameStateContext.ChangeHandlerEasier(gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Error), networkCommandConnectionToHandle);
                    return;
                }
                if (!CachedAuthorizedUsers.TryAdd(networkCommandConnectionToHandle.ConnectionId, userData))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Whoops! Could not add user to cached authorized users");
                    gameStateContext.ChangeHandlerEasier(gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Error), networkCommandConnectionToHandle);
                    return;
                }

                //say to client ready for rsc, waiting for signal
                gameStateContext.CurrentNetOutgoingMessageBusService.SendToClient(
                    CLINetworkCommandConstants.WaitForSignalCommand,
                    gameStateContext.GameStateRegistrar.WorldGameStateDataLayer.GetGameStateByLoginToken(userData?.LoginToken),
                    Lidgren.Network.NetDeliveryMethod.ReliableOrdered,
                    0,
                    networkCommandConnectionToHandle.Connection);
            }
            else
            {
                //this is only triggered if the user has been registered as a CLI user
                gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.CLIPassthrough).Handle(gameStateContext, networkCommandConnectionToHandle);
            }
        }

    }
}