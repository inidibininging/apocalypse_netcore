﻿using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Network;
using Apocalypse.Any.Infrastructure.Server.States.Interfaces;
using Echse.Net.Domain;
using Lidgren.Network;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Apocalypse.Any.Infrastructure.Server.States
{
    public class ServerNetworkGameStateContext<TWorld> : INetworkStateContext<TWorld>
        where TWorld : class, IWorldGameStateDataIOLayer, IWorldGameSectorInputLayer
    {
        public IGameStateService<byte,TWorld> GameStateRegistrar { get; }

        //public INetworkCommandConnectionToGameStateTranslatorService NetworkCommandConnectionToGameStateTranslators { get; set; }
        private NetIncomingMessageBusService<NetServer> CurrentNetIncomingMessageBusService { get; }

        public NetOutgoingMessageBusService<NetServer> CurrentNetOutgoingMessageBusService { get; }

        private IInputTranslator<NetIncomingMessage, NetworkCommandConnection> CurrentNetworkCommandServerTranslator { get; set; }

        public ILogger<byte> Logger { get; private set; }


        private readonly ConcurrentDictionary<long, INetworkLayerState<TWorld>> clientHandlers = new ConcurrentDictionary<long, INetworkLayerState<TWorld>>();

        public INetworkLayerState<TWorld> this[long clientIdentifier]
        {
            get
            {
                if (!clientHandlers.ContainsKey(clientIdentifier) && !clientHandlers.TryAdd(clientIdentifier, clientHandlers[(byte)ServerInternalGameStates.Login]))
                {
                    throw new InvalidOperationException("try adding a new client");
                }

                return clientHandlers[clientIdentifier];
            }
            private set
            {
                clientHandlers[clientIdentifier] = value;
            }
        }

        public ServerNetworkGameStateContext(
            // NetIncomingMessageBusService<NetServer> netIncomingMessageBusService,
            NetOutgoingMessageBusService<NetServer> netOutgoingMessageBusService,
            IInputTranslator<NetIncomingMessage, NetworkCommandConnection> networkCommandServerTranslator,
            IGameStateService<byte,TWorld> gameStateService,
            ILogger<byte> logger)
        {
            // CurrentNetIncomingMessageBusService = netIncomingMessageBusService ?? throw new ArgumentNullException(nameof(netIncomingMessageBusService));
            CurrentNetOutgoingMessageBusService = netOutgoingMessageBusService ?? throw new ArgumentNullException(nameof(netOutgoingMessageBusService));
            CurrentNetworkCommandServerTranslator = networkCommandServerTranslator ?? throw new ArgumentNullException(nameof(networkCommandServerTranslator));            
            GameStateRegistrar = gameStateService ?? throw new ArgumentNullException(nameof(gameStateService));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Initialize()
        {
            clientHandlers.TryAdd((byte)ServerInternalGameStates.Initial, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Initial));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.Login, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Login));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.LoginSuccessful, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.LoginSuccessful));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.ReceiveWork, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.ReceiveWork));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.SendPressedRelease, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.SendPressedRelease));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.CLIPassthrough, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.CLIPassthrough));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.Undefined, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Undefined));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.Error, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Error));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.Update, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.Update));
            clientHandlers.TryAdd((byte)ServerInternalGameStates.UpdateDelta, GameStateRegistrar.GetNetworkLayerState((byte)ServerInternalGameStates.UpdateDelta));
        }

        public void ForwardIncomingMessagesToHandlers(List<NetIncomingMessage> messageChunk)
        {          
            messageChunk?
            .ForEach(message => 
            {
                //Important note: The server will try to establish a connection externally. If the connection doesnt work it depends on a fail net connection.
                //For example: I had huge problems connecting after recognizing that the client and server didnt connect cuz my wifi was broken. -.-
                if (message.MessageType == NetIncomingMessageType.Data)
                {
                    var networkCommandConnection = CurrentNetworkCommandServerTranslator.Translate(message);
                    this[networkCommandConnection.Connection.RemoteUniqueIdentifier].Handle(this, networkCommandConnection);
                }
            });
        }



        public void ChangeHandlerEasier(INetworkLayerState<TWorld> gameState, NetworkCommandConnection networkCommandConnection)
        {
            Logger.LogDebug($"{gameState.ToString()} - {networkCommandConnection.Connection.Statistics}" );
            this[networkCommandConnection.Connection.RemoteUniqueIdentifier] = gameState;
        }

        
    }
}