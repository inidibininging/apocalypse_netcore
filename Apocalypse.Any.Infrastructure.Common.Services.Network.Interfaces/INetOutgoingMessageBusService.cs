using Lidgren.Network;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces
{
    public interface INetOutgoingMessageBusService
    {
        NetOutgoingMessage CreateMessage<T>(T instanceToSend);

        //NetSendResult SendMessage(NetOutgoingMessage message, NetConnection netConnection);
        NetSendResult SendToClient<T>(string commandName, T instanceToSend, NetConnection netConnection);
    }
}