using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Core.Utilities;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Common.Network;
using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using Newtonsoft.Json;
using System;
using System.Linq;
using Echse.Net.Serialization;

namespace Apocalypse.Any.Infrastructure.Server.States.Translators
{
    public class NetworkCommandToUserDataWithLoginToken : IInputTranslator<NetworkCommandConnection, UserDataWithLoginToken>
    {
        private IUserLoginService LoginService { get; set; }
        private IUserDataRoleService RoleService { get; set; }
        public IByteArraySerializationAdapter SerializationAdapter { get; }

        public NetworkCommandToUserDataWithLoginToken(
            IUserLoginService loginService,
            IUserDataRoleService roleService,
            IByteArraySerializationAdapter serializationAdapter)
        {
            if (loginService == null)
                throw new ArgumentNullException(nameof(loginService));
            LoginService = loginService;
            if (roleService == null)
                throw new ArgumentNullException(nameof(roleService));
            RoleService = roleService;
            SerializationAdapter = serializationAdapter ?? throw new ArgumentNullException(nameof(serializationAdapter));
        }

        private bool HasValidGameStateData(NetworkCommandConnection networkCommandConnection)
        {
            if (networkCommandConnection == null)
                return false;
            if (networkCommandConnection.CommandName != NetworkCommandConstants.LoginCommand)
                return false;
            if (networkCommandConnection.Data == null)
                return false;
            return true;
        }

        public UserDataWithLoginToken Translate(NetworkCommandConnection input)
        {
            if (!HasValidGameStateData(input))
                return null;

            var types = input.CommandArgument.LoadType(true, false);
            var userData = SerializationAdapter.DeserializeObject(input.Data, types.FirstOrDefault()) as UserData;

            if (userData == null)
                throw new ArgumentNullException(nameof(UserData));

            var userRole = RoleService.GetRoles(userData);
            var loginToken = LoginService.GetLoginToken(userData);

            return new UserDataWithLoginToken()
            {
                LoginToken = loginToken,
                Username = userData.Username,
                Password = userData.Password,
                Roles = userRole
            };
        }
    }
}