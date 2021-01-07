using Apocalypse.Any.Client.Screens;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;
using States.Core.Infrastructure.Services;
using System;
using System.Threading.Tasks;

namespace Apocalypse.Any.Client.States
{
    /// <summary>
    /// State for login in to the server.
    /// This state will try every X seconds if the attempted login failed.
    /// </summary>
    public class LoginState : IState<string, INetworkGameScreen>
    {
        private UserData PlayerData { get; set; }
        private string ServerIp { get; set; } = "127.0.0.1";
        private int ServerPort { get; set; } = 8080;
        public IByteArraySerializationAdapter SerializationAdapter { get; }
        public int SecondsToNextLoginTry { get; private set; } = 1;

        public LoginState(UserData playerData, string ip, int port, IByteArraySerializationAdapter serializationAdapter)
        {
            if (string.IsNullOrEmpty(ip))
            {
                throw new ArgumentException("message", nameof(ip));
            }

            PlayerData = playerData ?? throw new ArgumentNullException(nameof(playerData));
            ServerIp = ip;
            ServerPort = port;
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }

        private byte[] CreateMessage<T>(byte commandName, T instanceToSend)
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

        public void Handle(IStateMachine<string, INetworkGameScreen> machine)
        {
            machine.SharedContext.Messages.Add(nameof(LoginState));

            ServerIp = machine.SharedContext.Configuration.ServerIp;
            ServerPort = machine.SharedContext.Configuration.ServerPort;
            PlayerData = machine.SharedContext.Configuration.User;
            SecondsToNextLoginTry = machine.SharedContext.SecondsToNextLoginTry;

            //send login
            while (machine.SharedContext.LoginSendResult != NetSendResult.Sent)
            {
                machine.SharedContext.Messages.Add($"Sending.Try Nr:{machine.SharedContext.LoginTries}");
                var user = CreateMessage(NetworkCommandConstants.LoginCommand, machine.SharedContext.Configuration.User);
                var outgoingMessage = machine.SharedContext.Client.CreateMessage();
                outgoingMessage.Write(user);
                machine.SharedContext.LoginSendResult = machine.SharedContext.Client.SendMessage(outgoingMessage, NetDeliveryMethod.Unreliable);

                machine.SharedContext.Messages.Add($"Wait {SecondsToNextLoginTry} seconds...");
                Task.Delay(TimeSpan.FromSeconds(SecondsToNextLoginTry)).Wait();
                machine.SharedContext.LoginTries++;
            }
        }
    }
}