using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using Echse.Net.Serialization;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network
{
    /// <summary>
    /// Converts any NetworkCommand Data to an instance of object. If the object type is not registered, an ArgumentNullException wil be thrown
    /// </summary>
    public class NetworkCommandDataConverterService
    {
        
        public NetworkCommandDataConverterService(IByteArraySerializationAdapter serializationAdapter)
        {
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }

        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public object ConvertToObject(NetworkCommand command)
        {
            //command.CommandArgument
            
            var types = command.CommandArgument.LoadType(false, false);
            // if (types.Any())
            //     types = new Type[] { command.CommandArgument.GetApocalypseTypes() };

            var metaDataTyped = SerializationAdapter.DeserializeObject(command.Data, types.FirstOrDefault());

            if (metaDataTyped == null)
                throw new ArgumentNullException(nameof(metaDataTyped));

            return metaDataTyped;
        }
    }
}