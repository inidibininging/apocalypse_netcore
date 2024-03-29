﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;

namespace Apocalypse.Any.GameServer.Delegator.Delegation
{
    public class UserDataDelegationService
    {
        private NetClient Client { get; set; }
        public ConcurrentBag<string> Messages { get; private set; }
        private string Token;
        private bool Feeding { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public UserDataDelegationService(IByteArraySerializationAdapter serializationAdapter)
        {
            Messages = new ConcurrentBag<string>();
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
        public byte[] Feed()
        {
            Feeding = true;
            //Thread.Sleep(TimeSpan.FromMilliseconds(250));

            var currentMessage = Client.ReadMessage();


            if (currentMessage == null)
                return null;
                
            if (currentMessage.MessageType != NetIncomingMessageType.Data)
                return null;

            byte[] package = null;
            var packageLength = currentMessage.LengthBytes;
            try
            {
                package = currentMessage.ReadBytes(packageLength);

                if (package.Length != 0)
                {
                    if (Messages.Count > 64)
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("MEMORY RESET");
                        Console.ForegroundColor = ConsoleColor.White;
                        Messages = new ConcurrentBag<string>();
                    }

                    //Messages.Add(package);
                }


                //Console.WriteLine(package);

                dynamic serialized = SerializationAdapter.DeserializeObject<object>(package);
                dynamic data = serialized.Data;

                dynamic dataSerialized = SerializationAdapter.DeserializeObject<object>(data.Value);
                foreach(var prop in dataSerialized.Properties())
                    if(prop.Name == "LoginToken")
                        Token = prop.Value;


            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if(!string.IsNullOrWhiteSpace(Token))
                Client.SendMessage(
                                CreateMessage(
                                    NetworkCommandConstants.UpdateCommand,
                                    new GameStateUpdateData()
                                    {
                                        LoginToken = Token,
                                        Commands = new List<string>()
                                    }),
                                NetDeliveryMethod.ReliableOrdered);

            return package;
        }

        public void TryLogin(string peerName, string serverIp, int port, string username, string password, int trySecondsInterval = 1)
        {
            if (Feeding)
                return;
            Client = new NetClient(new NetPeerConfiguration(peerName)
            {
                EnableUPnP = true,
                AutoFlushSendQueue = true
            });
            Client.Start();
            Client.Connect(serverIp, port);
            NetSendResult res = NetSendResult.FailedNotConnected;
            var userData = new UserData() { Username = username, Password = password };
            //var serializedUsr = JsonConvert.SerializeObject(userData);

            //send login
            int loginTries = 1;
            int secondsLoginTry = trySecondsInterval;
            while (res != NetSendResult.Sent)
            {
                Console.WriteLine($"Sending.Try Nr:{loginTries}");
                res = Client.SendMessage
                    (
                        CreateMessage(NetworkCommandConstants.LoginCommand,
                                        userData),
                                        NetDeliveryMethod.ReliableOrdered
                    );

                Console.WriteLine($"Wait {secondsLoginTry} seconds...");
                Task.Delay(TimeSpan.FromSeconds(secondsLoginTry)).Wait();
                loginTries++;
            }


            Thread feedThread = new Thread(() => {

                while(true)
                    Feed();

                }) { IsBackground = true };
            feedThread.Start();
        }
    }
}
