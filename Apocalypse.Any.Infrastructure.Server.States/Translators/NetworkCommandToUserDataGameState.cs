using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Apocalypse.Any.Infrastructure.Server.States.Translators
{
    public class NetworkCommandToUserDataGameState : INetworkCommandConnectionToGameStateTranslator
    {
        private IUserLoginService LoginService { get; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public NetworkCommandToUserDataGameState(IUserLoginService loginService, IByteArraySerializationAdapter serializationAdapter)
        {
            LoginService = loginService ?? throw new ArgumentNullException(nameof(loginService));
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }

        private bool HasValidGameStateData(NetworkCommandConnection networkCommandConnection)
        {
            if (networkCommandConnection == null)
                return false;
            if (networkCommandConnection.CommandName != NetworkCommandConstants.LoginCommand)
                return false;
            if (networkCommandConnection.Data.Length == 0)
            {
                Console.WriteLine("Warning. Data is 0");
                return false;
            }
            return true;
        }

        public GameStateData Translate(NetworkCommandConnection input)
        {
            if (!HasValidGameStateData(input))
                throw new ArgumentException($"{nameof(input)} has no valid network command connection");

            Console.WriteLine("casting to type...");
            var types = input.CommandArgument.LoadType(true, false);
            if (!(SerializationAdapter.DeserializeObject(input.Data, types.FirstOrDefault()) is UserData userData))
                throw new ArgumentNullException(nameof(UserData));

            Console.WriteLine($"get login token... for {userData.Username}");
            string loginToken = LoginService.GetLoginToken(userData);

            return new GameStateData()
            {
                Id = Guid.NewGuid().ToString(),
                LoginToken = loginToken
            };
        }
    }
}