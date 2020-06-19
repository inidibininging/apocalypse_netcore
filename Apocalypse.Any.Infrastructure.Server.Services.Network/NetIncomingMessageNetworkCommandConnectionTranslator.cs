using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;

//using Apocalypse.Any.Domain.Server.Model.Network;
using Lidgren.Network;
using System;

namespace Apocalypse.Any.Core.Network.Server.Services
{
    /// <summary>
    /// Translates net incoming messages to network command connection objects
    /// </summary>
    public class NetIncomingMessageNetworkCommandConnectionTranslator : IInputTranslator<NetIncomingMessage, NetworkCommandConnection>
    {
        private NetworkCommandTranslator IncomingMessageTranslator { get; set; }

        public NetIncomingMessageNetworkCommandConnectionTranslator(NetworkCommandTranslator networkCommandTranslator)
        {
            if (networkCommandTranslator == null)
                throw new ArgumentNullException(nameof(networkCommandTranslator));
            IncomingMessageTranslator = networkCommandTranslator;
        }

        public NetworkCommandConnection Translate(NetIncomingMessage input)
        {
            if (input == null)
                return null;
            var tzeMessisch = input.ReadString();
            if (string.IsNullOrWhiteSpace(tzeMessisch))
                return null;

            NetworkCommand networkCommand = IncomingMessageTranslator.Translate(tzeMessisch);
            //Console.WriteLine(networkCommand.Data);
            NetworkCommandConnection networkCommandConnection = new NetworkCommandConnection();

            //Conversion to network command on server
            networkCommandConnection.Connection = input.SenderConnection;
            networkCommandConnection.CommandArgument = networkCommand.CommandArgument;
            networkCommandConnection.CommandName = networkCommand.CommandName;
            networkCommandConnection.Data = networkCommand.Data;

            return networkCommandConnection;
        }
    }
}