using System;
using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Lidgren.Network; //using Apocalypse.Any.Domain.Server.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Network
{
    /// <summary>
    /// Translates net incoming messages to a network command connection object
    /// </summary>
    public class NetIncomingMessageNetworkCommandConnectionTranslator : IInputTranslator<NetIncomingMessage, NetworkCommandConnection>
    {
        private NetworkCommandTranslator IncomingMessageTranslator { get; set; }

        public NetIncomingMessageNetworkCommandConnectionTranslator(NetworkCommandTranslator networkCommandTranslator)
        {
            IncomingMessageTranslator = networkCommandTranslator ?? throw new ArgumentNullException(nameof(networkCommandTranslator));
        }

        public NetworkCommandConnection Translate(NetIncomingMessage input)
        {
            if (input == null)
                return null;
            
            var messageAsBytes = input.ReadBytes(input.LengthBytes);
            if (messageAsBytes == null || messageAsBytes?.Length == 0)
            {
                //TODO: return a network command connection as error
                return null;
            }

            NetworkCommand networkCommand = IncomingMessageTranslator.Translate(messageAsBytes);
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