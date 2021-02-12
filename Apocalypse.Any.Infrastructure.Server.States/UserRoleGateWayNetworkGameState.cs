using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class UserRoleGateWayNetworkGameState<TWorld> : INetworkLayerState<TWorld>
        where TWorld : IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private NetworkCommandDataConverterService NetworkCommandDataConverterService { get; }
        
        private IUserDataRoleService UserDataRoleService { get; }

        private Dictionary<UserDataRole, byte> Routes { get; set; } = new Dictionary<UserDataRole, byte>()
        {
            { UserDataRole.CanReceiveWork, (byte)ServerInternalGameStates.ReceiveWork },
            { UserDataRole.CanSendRemoteStateCommands, (byte)ServerInternalGameStates.CLIPassthrough },
            { UserDataRole.CanSendRemoteMovementCommands, (byte)ServerInternalGameStates.SendPressedRelease },
            { UserDataRole.CanViewWorldByLoginToken, (byte)ServerInternalGameStates.Update },
        };

        public UserRoleGateWayNetworkGameState(IUserDataRoleService userDataRoleService, NetworkCommandDataConverterService networkCommandDataConverterService)
        {
            UserDataRoleService = userDataRoleService ?? throw new ArgumentNullException(nameof(userDataRoleService));
            NetworkCommandDataConverterService = networkCommandDataConverterService ?? throw new ArgumentNullException(nameof(networkCommandDataConverterService));
        }
        public void Handle(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            var typeToProofAgainst = typeof(UserData).FullName;

            //Now we look what the roles are and deliver answers
            if (networkCommandConnectionToHandle.CommandArgument != typeToProofAgainst)
            {
                ThrowError(gameStateContext, networkCommandConnectionToHandle);
            }

            var convertedInstance = NetworkCommandDataConverterService.ConvertToObject(networkCommandConnectionToHandle);
            if(convertedInstance as UserData == null)
            {
                ThrowError(gameStateContext, networkCommandConnectionToHandle);
            }

            //Actual router. Only one state will fire. See that the order of the entries in Router also provides the priority. 
            //TODO: this should be a feature/library in the future
            var userRoles = UserDataRoleService.GetRoles(convertedInstance as UserData);
            var userRolesHandler = gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)Routes.FirstOrDefault(kv => userRoles.HasFlag(kv.Key)).Value);
            
            gameStateContext.Logger.Log(LogLevel.Information, $"{nameof(UserRoleGateWayNetworkGameState<TWorld>)} performing ChangeHandlerEasier {userRolesHandler}");
            gameStateContext.ChangeHandlerEasier(userRolesHandler, networkCommandConnectionToHandle);
            
            userRolesHandler.Handle(gameStateContext, networkCommandConnectionToHandle);
        }
        private void ThrowError(INetworkStateContext<TWorld> gameStateContext, NetworkCommandConnection networkCommandConnectionToHandle)
        {
            gameStateContext.Logger.Log(LogLevel.Error, $"Cannot transition user to update state. Command argument is not of type user data. Type is {networkCommandConnectionToHandle.CommandArgument}");
            gameStateContext.ChangeHandlerEasier(gameStateContext.GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Error), networkCommandConnectionToHandle);
        }
    }
}
