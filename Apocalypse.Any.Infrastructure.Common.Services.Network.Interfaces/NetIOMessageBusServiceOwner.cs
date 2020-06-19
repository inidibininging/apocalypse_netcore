namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces
{
    public interface NetIOMessageBusServiceOwner
    {
        INetIncomingMessageBusService InputBus { get; }
        INetOutgoingMessageBusService OutputBus { get; }
    }
}