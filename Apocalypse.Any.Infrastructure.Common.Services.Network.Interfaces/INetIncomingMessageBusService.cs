using Lidgren.Network;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces
{
    public interface INetIncomingMessageBusService
    {
        IList<NetIncomingMessage> FetchMessageChunk();

        Task<IList<NetIncomingMessage>> FetchMessageChunkAsync();
    }
}