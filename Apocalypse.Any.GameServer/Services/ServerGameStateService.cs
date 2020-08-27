using Apocalypse.Any.Infrastructure.Common.Services.Data;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Apocalypse.Any.Infrastructure.Server.States.Translators;
using System;
using System.Collections.Concurrent;

namespace Apocalypse.Any.GameServer.Services
{
    /// <summary>
    /// Dirty code that needs to be cleaned. For now, it's a dependency trash can in order to use some game world dependencies inside the network layer
    /// </summary>
    public class ServerGameStateService<TWorld> : IGameStateService<byte, TWorld>
        where TWorld : class, IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        private ConcurrentDictionary<byte, INetworkLayerState<TWorld>> InternalGameStates { get; set; } = new ConcurrentDictionary<byte, INetworkLayerState<TWorld>>();

        public TWorld WorldGameStateDataLayer { get; private set; }
        public ISerializationAdapter SerializationAdapter { get; }
        public IUserLoginService LoginService { get; private set; }
        public IUserDataRoleService RoleService { get; private set; }

        public ServerGameStateService(
            IUserLoginService loginService,
            IUserDataRoleService roleService,
            TWorld gameStateDataLayer,
            ISerializationAdapter serializationAdapter)
        {
            LoginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            RoleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
            WorldGameStateDataLayer = gameStateDataLayer ?? throw new ArgumentNullException(nameof(gameStateDataLayer));
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
            LoadDefaultGameStates();
        }

        private void LoadDefaultGameStates()
        {
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.Initial, new InitialNetworkGameState<TWorld>(new NetworkCommandToInitialGameState(WorldGameStateDataLayer, SerializationAdapter)));
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.Login, new LoginNetworkGameState<TWorld>(new NetworkCommandToUserDataGameState(LoginService, SerializationAdapter)));
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.LoginSuccessful, new LoginSuccessfulToUpdateGameState<TWorld>(new NetworkCommandToUserDataGameState(LoginService, SerializationAdapter)));
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.CLI, new CLIGameState<TWorld>(new NetworkCommandToUserDataWithLoginToken(LoginService, RoleService, SerializationAdapter)));
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.CLIPassthrough, new CLIPassthroughGameState<TWorld>());
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.Update, new UpdateNetworkGameState<TWorld>(new NetworkCommandToUpdateGameState(WorldGameStateDataLayer, SerializationAdapter),
                                                                                                          SerializationAdapter));
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.UpdateDelta, new UpdateDeltaNetworkGameState<TWorld>(new NetworkCommandToUpdateGameState(WorldGameStateDataLayer, SerializationAdapter),
                                                                                                          SerializationAdapter,
                                                                                                          new DeltaGameStateDataService()));
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.Undefined, new ErrorNetworkGameState<TWorld>());
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.ReceiveWork, new ReceiveWorkNetworkGameState<TWorld>(new NetworkCommandDataConverterService(SerializationAdapter)));
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.UserRoleGateWay, new UserRoleGateWayNetworkGameState<TWorld>(RoleService, new NetworkCommandDataConverterService(SerializationAdapter)));
            InternalGameStates.GetOrAdd((byte)ServerInternalGameStates.Error, new ErrorNetworkGameState<TWorld>());
        }

        public INetworkLayerState<TWorld> GetNeworkLayerState(byte identifier)
        {
            if (!InternalGameStates.ContainsKey(identifier))
                throw new NotImplementedException();
            return InternalGameStates[identifier];
        }
    }
}