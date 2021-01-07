using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Lidgren.Network;
using Newtonsoft.Json;

namespace Apocalypse.Any.Domain.Common.Network.Utilities
{
    public static class NetworkExtensions
    {
        /// <summary>
        /// Serializes an object of type T to a client message
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stuff"></param>
        /// <returns></returns>
        public static NetOutgoingMessage ToClientOutgoingMessage<T>(this T stuff, NetClient client, IStringSerializationAdapter serializationAdapter)
        {
            return client.CreateMessage(serializationAdapter.SerializeObject(stuff));
        }
    }
}