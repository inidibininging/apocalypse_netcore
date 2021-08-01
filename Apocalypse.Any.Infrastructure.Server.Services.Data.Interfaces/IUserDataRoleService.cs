using System.Collections.Generic;
using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Domain.Server.Model.Network;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IUserDataRoleService
    {
        Dictionary<UserDataRoleSource,UserDataRole> GetRoles(UserData userData);
    }
}