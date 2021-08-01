using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Echse.Net.Serialization;

namespace Apocalypse.Any.CLI
{
    public class CLIServerConnector
    {
        public GameStateData Data { get; set; }
        private NetClient Client { get; set; }
        public UserData User { get; set; }
        public string ServerIp { get; set; }
        public int ServerPort { get; set; }

        public string LoginToken { get; set; }
        public int SecondsToNextLoginTry { get; set; } = 1;
        public Queue<string> Commands { get; private set; } = new Queue<string>();
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public CLIServerConnector(IByteArraySerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }
        private NetOutgoingMessage CreateMessage<T>(byte commandName, T instanceToSend)
        {
            var msg = Client.CreateMessage();
            msg.Write(SerializationAdapter.SerializeObject
                    (
                        new NetworkCommand()
                        {
                            CommandName = commandName,
                            CommandArgument = typeof(T).FullName,
                            Data = SerializationAdapter.SerializeObject(instanceToSend)
                        }
                    ));
            return msg;
        }

        public void Initialize()
        {
            var serializedUsr = User;

            var peerName = "asteroid";
            Client = new NetClient(new NetPeerConfiguration(peerName)
            {
                EnableUPnP = true,
                AutoFlushSendQueue = true
            });

            Client.Start();
            Client.Connect(ServerIp, ServerPort);
            NetSendResult res = NetSendResult.FailedNotConnected;

            //send login
            while (res != NetSendResult.Sent)
            {
                res = Client.SendMessage
                    (
                        CreateMessage(NetworkCommandConstants.LoginCommand,
                                        serializedUsr),
                                        NetDeliveryMethod.Unreliable
                    );

                Task.Delay(TimeSpan.FromSeconds(SecondsToNextLoginTry)).Wait();
            }
        }

        public void Update()
        {
            var currentMessage = Client.ReadMessage();
            if (currentMessage == null)
                return;

            //Console.WriteLine(currentMessage.MessageType);
            if (currentMessage.MessageType != NetIncomingMessageType.Data)
                return;

            try
            {
                var readMsgLength = currentMessage.LengthBytes;
                var readMsg = currentMessage.ReadBytes(readMsgLength);
                var netCmd = SerializationAdapter.DeserializeObject<IdentifiableNetworkCommand>(readMsg);
                var gData = SerializationAdapter.DeserializeObject<GameStateData>(netCmd.Data);

                if (gData != null)
                    Data = gData;
                if (string.IsNullOrWhiteSpace(LoginToken))
                    LoginToken = gData.LoginToken;

                var sendResult = Client.SendMessage(
                                    CreateMessage(
                                        NetworkCommandConstants.UpdateCommand,
                                        new GameStateUpdateData()
                                        {
                                            LoginToken = this.LoginToken,
                                            Commands = Commands.TryPeek(out string someCommand) ? new List<string>() { Commands.Dequeue() } : null
                                        }),
                                    NetDeliveryMethod.UnreliableSequenced);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message + ex.InnerException != null ? (System.Environment.NewLine + ex.InnerException.Message) : "");
            }
        }
    }
}