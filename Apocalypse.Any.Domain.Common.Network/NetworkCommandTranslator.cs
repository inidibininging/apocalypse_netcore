using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Newtonsoft.Json;
using System;
using Echse.Net.Serialization;

namespace Apocalypse.Any.Domain.Common.Network
{
    /// <summary>
    /// Deserializes a string to a network command object
    /// </summary>
    public class NetworkCommandTranslator : IInputTranslator<byte[], NetworkCommand>
    {
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public NetworkCommandTranslator(IByteArraySerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter;
        }
        /// <summary>
        /// Translates the string input into a serialized Object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public NetworkCommand Translate(byte[] input)
        {
            if (input.Length == 0)
                throw new NoValidNetworkCommandException();
            return SerializationAdapter.DeserializeObject<NetworkCommand>(input);
        }
    }
}