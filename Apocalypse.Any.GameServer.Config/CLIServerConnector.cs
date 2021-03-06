﻿using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Lidgren.Network;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apocalypse.Any.GameServer.Config
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

        private NetOutgoingMessage CreateMessage<T>(string commandName, T instanceToSend)
        {
            return Client.CreateMessage(

                    JsonConvert.SerializeObject
                    (
                        new NetworkCommand()
                        {
                            CommandName = commandName,
                            CommandArgument = typeof(T).FullName,
                            Data = JsonConvert.SerializeObject(instanceToSend)
                        }
                    )
                );
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
                var readMsg = currentMessage.ReadString();
                var netCmd = JsonConvert.DeserializeObject<IdentifiableNetworkCommand>(readMsg);
                var gData = JsonConvert.DeserializeObject<GameStateData>(netCmd.Data);

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