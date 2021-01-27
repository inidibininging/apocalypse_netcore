using Apocalypse.Any.Core.Network.Server.Services;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Client.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.DataLayer;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Threading;
using System.Threading.Tasks;

namespace Apocalypse.Any.Infrastructure.Server.Worker
{
    /// <summary>
    /// A data layer worker only works with information based on the data layer. The data is withold in memory for best perfomance.
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
        private GameClientConfiguration ClientConfiguration { get; set; }

        private IGameSectorDataLayer<
            TPlayer,
            TEnemy,
            TItem,
            TProjectile,
            TEntitiesBaseType,
            TGeneralCharacter,
            TImageData> DataLayer
        { get; set; }

        private NetIncomingMessageBusService<NetClient> Input { get; set; }
        private NetOutgoingMessageBusService<NetClient> Output { get; set; } // not in use for now cause it doesnt work? WHY?
        private NetworkCommandDataConverterService NetworkCommandDataConverterService { get; set; }
        private NetIncomingMessageNetworkCommandConnectionTranslator NetIncomingMessageNetworkCommand { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; set; }

        public string LoginToken { get; private set; }
        public SyncClient(GameClientConfiguration configuration)
        {
            ClientConfiguration = configuration;
            CreateClientAndConnect();

            // PerformFakeRelease();
        }

        // private void PerformFakeRelease() => Task.Factory.StartNew(() =>
        // {
        //     Thread.Sleep(15.Seconds());
        //     doIt = true;
        // });
        private Type GetSerializer(string serializerType)
        {
            string baseNameSpace = "Apocalypse.Any.Infrastructure.Common.Services.Serializer*.dll";
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

        public bool TryLogin()
        {
            var message = CreateMessageContent(NetworkCommandConstants.LoginCommand, ClientConfiguration.User);
            if (Client.ConnectionStatus != NetConnectionStatus.Connected)
                Client.Connect(ClientConfiguration.ServerIp, ClientConfiguration.ServerPort);
            return (Client.SendMessage(
                CreateMessage(Client, message),
                NetDeliveryMethod.ReliableOrdered) == NetSendResult.Sent);
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

        public Queue<int> Commands { get; set; }
        public void ProcessIncomingMessages(IEnumerable<int> commands)
        {
            foreach(var cmd in commands)
                Commands.Enqueue(cmd);

            var nextCommand = Commands.Count == 0 ? -1 : Commands.Dequeue();
            var dataInputs = Input
                            .FetchMessageChunk()
                            .Where(msg => msg.MessageType == NetIncomingMessageType.Data);

            var inputs = dataInputs
                            .Select(msg =>
                            {
                                try
                                {
                                    return NetIncomingMessageNetworkCommand.Translate(msg);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                                return null;
                            })
                            .Where(msg => msg != null && 
                                          (msg.CommandName == NetworkCommandConstants.SendPressReleaseCommand || 
                                           msg.CommandName == NetworkCommandConstants.UpdateCommand)) // why do I receive gamestate data?? => because a player needs to be physically bound to a game .else other mechanics depending on getting the players game state for every registered player will fail.
                            .Select(msg =>
                            {
                                if (msg.Data == null || msg.Data.Length == 0)
                                    return true;
                                try
                                {
                                    Console.WriteLine(msg.Data);
                                    Console.WriteLine("--------------------------");
                                    //TODO: Pass a map of states mapped to bytes

                                    var ret = NetworkCommandDataConverterService.ConvertToObject(msg);

                                    switch (ret)
                                    {
                                        //Login successful, remember the login token
                                        case GameStateData gameStateData when !string.IsNullOrWhiteSpace(gameStateData.LoginToken):
                                            LoginToken = gameStateData.LoginToken;
                                            break;

                                        //ACK Response                                        
                                        case bool b when b && !AckReceived:
                                        {
                                            //send fake input command
                                            var fakeInput = Output.SendToClient(NetworkCommandConstants.SendPressReleaseCommand,
                                                new PressReleaseUpdateData() { Command = Commands.Count == 0 ? -1 : Commands.Dequeue(), LoginToken = LoginToken},
                                                NetDeliveryMethod.ReliableOrdered, 0, msg.Connection);
                                            AckReceived = true;
                                            Console.WriteLine("Ack received");
                                            //TODO: Send release after some time
                                            break;
                                        }
                                    }

                                    Console.WriteLine("--------------------------");
                                    return ret;
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine(ex);
                                }
                                return null;
                            })
                            .ToList();

            if (AckReceived)
            {

                var serverConnection = Client.Connections.FirstOrDefault();
                var fakeInput = Output.SendToClient(NetworkCommandConstants.SendPressReleaseCommand,
                    new PressReleaseUpdateData() { Command = nextCommand, LoginToken = LoginToken},
                    NetDeliveryMethod.ReliableOrdered, 0, serverConnection); // HARD CODED connection. First one should be the one from the message sending the fake press
                Console.WriteLine($"Sent command {nextCommand}");
            }
        }
    }
}
