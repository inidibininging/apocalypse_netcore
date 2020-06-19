using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IUserDataRoleService
    {
        UserDataRole GetRoles(UserData userData);
    }
}