using Apocalypse.Any.Domain.Common.Model.Network;

namespace Apocalypse.Any.Domain.Server.Model.Network
{
    public class UserDataWithLoginToken : UserData
    {
        public string LoginToken { get; set; }
        public UserDataRole Roles { get; set; }
        public bool NewInGame { get; set; }
    }
}