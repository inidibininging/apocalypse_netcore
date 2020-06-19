using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Services;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Newtonsoft.Json;
using System;

namespace Apocalypse.Any.Domain.Common.Network
{
    /// <summary>
    /// Deserializes a string to a network command object
    /// </summary>
    public class NetworkCommandTranslator : IInputTranslator<string, NetworkCommand>
    {
        public ISerializationAdapter SerializationAdapter { get; }

        public NetworkCommandTranslator(ISerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter;
        }
        /// <summary>
        /// Translates the string input into a serialized Object
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public NetworkCommand Translate(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                throw new NoValidNetworkCommandException();
            return SerializationAdapter.DeserializeObject<NetworkCommand>(input);
        }
    }
}