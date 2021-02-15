﻿using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Infrastructure.Server.Services.Network;
using Microsoft.Extensions.Logging;

namespace Apocalypse.Any.Infrastructure.Server.Worker
{
    /// <summary>
    /// A connector that sends the current data inputs to the real server.
    /// If a desync happens the local server will attempt to sync with the real server 
    /// </summary>
    public class SyncClient<
        TPlayer,
        TEnemy,
        TItem,
        TProjectile,
        TEntitiesBaseType,
        TGeneralCharacter,
        TImageData>
    {
        private NetClient Client { get; set; }
        private GameClientConfiguration ClientConfiguration { get; }

        private ILogger<byte> Logger { get; }

        private GameStateDataLayer dataLayer;
        public GameStateDataLayer DataLayer
        {
            get {
                return dataLayer;
            }
            private set {
                dataLayer = value;
                NewDataLayer = true;
            }
        }

        private NetIncomingMessageBusService<NetClient> Input { get; set; }
        private NetOutgoingMessageBusService<NetClient> Output { get; set; } // not in use for now cause it doesnt work? WHY?
        private NetworkCommandDataConverterService NetworkCommandDataConverterService { get; set; }
        private NetIncomingMessageNetworkCommandConnectionTranslator NetIncomingMessageNetworkCommand { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; set; }

        public string LoginToken { get; private set; }

        private int lastSectorKey = -1;
        public int LastSectorKey
        {
            get
            {
                return lastSectorKey;
            }
            set
            {
                SectorChanged = lastSectorKey != value && lastSectorKey != -1;
                lastSectorKey = value;
            }
        }

        public bool SectorChanged { get; set; } = false;
        public bool NewDataLayer { get; set; } = false;

        public SyncClient(GameClientConfiguration configuration, ILogger<byte> logger)
        {
            ClientConfiguration = configuration;
            Logger = logger;
            CreateClientAndConnect();
        }

        private Type GetSerializer(string serializerType)
        {
            const string baseNameSpace = "Apocalypse.Any.Infrastructure.Common.Services.Serializer*.dll";
            // string currentDir = Directory.GetCurrentDirectory();
            string currentDir = System.IO.Path.GetDirectoryName(this.GetType().Assembly.Location);
            foreach (var file in Directory.EnumerateFiles(currentDir, baseNameSpace, SearchOption.TopDirectoryOnly))
            {
                try
                {
                    Console.WriteLine(file);
                    var leAssembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                    var leSerializerType = leAssembly.GetTypes().Where(t => t.FullName == serializerType);
                    var leType = leSerializerType.FirstOrDefault();
                    if (leType != null)
                        return leType;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("whoops");
                    Console.WriteLine(ex);
                }
            }
            return null;
        }

        private void CreateClientAndConnect()
        {
            Client = new NetClient(
                new NetPeerConfiguration(ClientConfiguration.ServerPeerName)
                {
                    AcceptIncomingConnections = true,
                    EnableUPnP = true,
                    AutoFlushSendQueue = true
                });
            Client.Start();
            Client.Connect(ClientConfiguration.ServerIp, ClientConfiguration.ServerPort);

            var serializerType = GetSerializer(ClientConfiguration.SerializationAdapterType);
            SerializationAdapter = Activator.CreateInstance(serializerType) as IByteArraySerializationAdapter;
            Input = new NetIncomingMessageBusService<NetClient>(Client);
            Output = new NetOutgoingMessageBusService<NetClient>(Client, SerializationAdapter);
            NetworkCommandDataConverterService = new NetworkCommandDataConverterService(SerializationAdapter);
            NetIncomingMessageNetworkCommand = new NetIncomingMessageNetworkCommandConnectionTranslator(new NetworkCommandTranslator(SerializationAdapter));
        }

        private void TryConnect()
        {
            if (Client == null)
            {
                Logger.LogWarning($"Tried to invoke {nameof(TryConnect)}, but there is no connection to a sync server");
                return;
            }

            if (Client.ConnectionStatus == NetConnectionStatus.Connected) return;
            Logger.LogInformation("############# Retry Connect ##############");
            Client.Connect(ClientConfiguration.ServerIp, ClientConfiguration.ServerPort);
        }
        public NetSendResult TryLogin()
        {
            if (Client == null) return NetSendResult.FailedNotConnected;

            var message = CreateMessageContent(NetworkCommandConstants.LoginCommand, ClientConfiguration.User);
            Logger.LogInformation("############# TryLogin ##############");
            return (Client.SendMessage(
                CreateMessage(Client, message),
                NetDeliveryMethod.ReliableOrdered));
        }

        private NetOutgoingMessage CreateMessage(NetPeer netPeer, byte[] content)
        {
            var msg = netPeer.CreateMessage();
                msg.Write(content);
            return msg;
        }
        private byte[] CreateMessageContent<T>(byte commandName, T instanceToSend)
        {
            var content = SerializationAdapter.SerializeObject
                    (
                        new NetworkCommand()
                        {
                            CommandName = commandName,
                            CommandArgument = typeof(T).FullName,
                            Data = SerializationAdapter.SerializeObject(instanceToSend)
                        }
                    );
            return content;
        }

        private bool AckReceived {get; set;} = false;
        public Queue<int> Commands { get; set; } = new Queue<int>();
        public void ProcessIncomingMessages(IEnumerable<int> commands)
        {
            TryConnect();

            foreach (var cmd in commands) {
                // Console.WriteLine($"enque:{cmd}");
                Commands.Enqueue(cmd);
            }

            var nextCommand = Commands.Count == 0 ? -1 : Commands.Dequeue();
            var dataInputs = Input
                            .FetchMessageChunk()
                            .Where(msg => msg.MessageType == NetIncomingMessageType.Data);

            dataInputs
                .ToList()
                // why do I receive gamestate data?? => because a player needs to be physically bound to a game .else other mechanics depending on getting the players game state for every registered player will fail.               
                .ForEach(msg =>
                {
                    NetworkCommandConnection networkCommandConnection = null;
                    try
                    {
                        networkCommandConnection = NetIncomingMessageNetworkCommand.Translate(msg);
                        Console.WriteLine(networkCommandConnection?.CommandName);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        return;
                    }

                    if (msg == null || networkCommandConnection == null)
                    {
                        Console.WriteLine("Message cannot be translated");
                        return;
                    }

                    if(networkCommandConnection.CommandName != NetworkCommandConstants.SendPressReleaseCommand && 
                       networkCommandConnection.CommandName != NetworkCommandConstants.UpdateCommand && 
                       networkCommandConnection.CommandName != NetworkCommandConstants.SyncSectorCommand)
                    {
                        Console.WriteLine($"Command name is not SendPressReleaseCommand, UpdateCommand or ReceiveWorkCommand -> {networkCommandConnection.CommandName}");
                        return;
                    }

                    if (msg.Data == null || msg.Data.Length == 0)
                    {
                        Console.WriteLine("Message data is null or length is zero");
                        return;
                    }
                    try
                    {
                        Console.WriteLine("--------------------------");
                        //TODO: Pass a map of states mapped to bytes

                        var returningGameObject = NetworkCommandDataConverterService.ConvertToObject(networkCommandConnection);
                        Console.WriteLine(returningGameObject);
                        switch (returningGameObject)
                        {
                            //Login successful, remember the login token used in sync server
                            case GameStateData gameStateData when !string.IsNullOrWhiteSpace(gameStateData.LoginToken):
                                LoginToken = gameStateData.LoginToken;
                                Logger.LogInformation("Login successful. Received game state data and login token");
                                ;
                                break;

                            //ACK Response                                        
                            case int b when b == NetworkCommandConstants.OutOfSyncCommand:
                                {
                                    Logger.LogInformation("ACK received!!! got OutOfSync");
                                    Logger.LogInformation(">>>>>>>>>>>>>>>>>>>>>>>>");
                                    //send first input command
                                    var syncSectorCommand = Output.SendToClient(NetworkCommandConstants.SyncSectorCommand,
                                        new ReceiveGameStateDataLayerPartRequest() {
                                            GetProperty = "DataLayer",
                                            SectorKey = LastSectorKey,
                                            // Command = Commands.Count == 0 ? -1 : Commands.Dequeue(),
                                            LoginToken = LoginToken
                                        },
                                        NetDeliveryMethod.ReliableOrdered, 0, networkCommandConnection.Connection);
                                    AckReceived = true;

                                    //TODO: Send release after some time
                                    break;
                                }

                            //GameStateDataLayer means that the client gets the actual data from the server
                            case GameStateDataLayer gameStateDataLayer when gameStateDataLayer != null && AckReceived:
                                {
                                    

                                    //TODO: forward game state of sync client to the local server                                    
                                    Logger.LogInformation("--------#---------------------#--------------");
                                    Logger.LogInformation("--------#-----ö---------ö-----#--------------");
                                    Logger.LogInformation("--------#----------f----------#--------------");
                                    Logger.LogInformation("--------#-----!!!!!!!!!!!!----#--------------");
                                    Logger.LogInformation("--------#---------------------#--------------");

                                    DataLayer = gameStateDataLayer;

                                    break;
                                }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });

            if (AckReceived && nextCommand != -1)
            {
                Console.WriteLine($"nextCommand:{nextCommand}");
                var serverConnection = Client.Connections.FirstOrDefault();
                var fakeInput = Output.SendToClient(NetworkCommandConstants.SendPressReleaseCommand,
                    new PressReleaseUpdateData() { Command = nextCommand, LoginToken = LoginToken, SectorKey = LastSectorKey },
                    NetDeliveryMethod.ReliableOrdered, 0, serverConnection); // HARD CODED connection. First one should be the one from the message sending the fake press
                Console.WriteLine($"Sent command {nextCommand}");
            }

            //Client knows that the sector changed
            //Tell the sync client "I want all of the new sector"
            if (AckReceived && SectorChanged)
            {
                Console.WriteLine("SECTOR CHANGED!");
                var serverConnection = Client.Connections.FirstOrDefault();
                var fakeInput = Output.SendToClient(NetworkCommandConstants.SyncSectorCommand,
                    new ReceiveGameStateDataLayerPartRequest()
                    {
                        LoginToken = LoginToken,
                        SectorKey = LastSectorKey,
                        GetProperty = "DataLayer" // If property is given, a part of the sector will be sent
                    },
                    NetDeliveryMethod.ReliableOrdered, 0, serverConnection); // HARD CODED connection. First one should be the one from the message sending the fake press
                SectorChanged = false;
                Console.WriteLine($"Sent command {nextCommand}");
            }
        }
    }
}