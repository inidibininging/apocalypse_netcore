using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Newtonsoft.Json;
using System;

namespace Apocalypse.Any.Core.Network.Server.Services
{
    public class NetworkCommandServerToLoginTokenTranslator : IInputTranslator<NetworkCommandConnection, string>
    {
        private IUserRegistrationService UserRegistration { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public NetworkCommandServerToLoginTokenTranslator(IUserRegistrationService userRegistrationDataLayer, IByteArraySerializationAdapter serializationAdapter)
        {
            if (userRegistrationDataLayer == null)
                throw new ArgumentNullException(nameof(userRegistrationDataLayer));
            UserRegistration = userRegistrationDataLayer;
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }

        public string Translate(NetworkCommandConnection input)
        {
            if (input == null)
                return null;
            if (input.CommandName != NetworkCommandConstants.LoginCommand)
                return null;
            if (input.CommandArgument != typeof(UserData).FullName)
                throw new InvalidOperationException("The specified type for login is not valid.");
            var userData = SerializationAdapter.DeserializeObject<UserData>(input.Data);
            if (userData == null)
                throw new InvalidOperationException("No user data specified");
            return UserRegistration.Register(userData);
        }
    }
}