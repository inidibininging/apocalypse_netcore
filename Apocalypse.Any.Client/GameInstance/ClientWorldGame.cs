using Apocalypse.Any.Client.States;
using Apocalypse.Any.Core;
using Apocalypse.Any.Core.Drawing;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apocalypse.Any.Client.GameInstance
{
    public class ClientWorldGame : IUpdateableLite, IVisualGameObject
    {
        public NetClient Client { get; set; }

        public ClientGameContext GameStateContext { get; set; }

        public string ServerIp { get; set; }
        public int ServerPort { get; set; }
        public UserData User { get; set; }
        public List<string> Messages { get; set; } = new List<string>();
        public int LoginTries { get; set; } = 0;
        public int SecondsToNextLoginTry { get; set; } = 1;
        public ISerializationAdapter SerializationAdapter { get; }

        public ClientWorldGame(ISerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }
        private NetOutgoingMessage CreateMessage<T>(string commandName, T instanceToSend)
        {
            return Client.CreateMessage(

                    SerializationAdapter.SerializeObject
                    (
                        new NetworkCommand()
                        {
                            CommandName = commandName,
                            CommandArgument = typeof(T).FullName,
                            Data = SerializationAdapter.SerializeObject(instanceToSend)
                        }
                    )
                );
        }

        public ClientWorldGame(string ip, int port, UserData loginData)
        {
            ServerIp = ip;
            ServerPort = port;
            User = loginData;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
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

            Messages.Add("Start status");
            //send login
            while (res != NetSendResult.Sent)
            {
                Messages.Add($"Sending.Try Nr:{LoginTries}");
                res = Client.SendMessage
                    (
                        CreateMessage(NetworkCommandConstants.LoginCommand,
                                        serializedUsr),
                                        NetDeliveryMethod.Unreliable
                    );

                Messages.Add($"Wait {SecondsToNextLoginTry} seconds...");
                Task.Delay(TimeSpan.FromSeconds(SecondsToNextLoginTry)).Wait();
                LoginTries++;
            }
        }

        public void LoadContent(ContentManager manager)
        {
            throw new NotImplementedException();
        }

        public void UnloadContent()
        {
            throw new NotImplementedException();
        }

        public void Update(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}