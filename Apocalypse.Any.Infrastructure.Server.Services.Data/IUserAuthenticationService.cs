﻿using Apocalypse.Any.Domain.Server.Model.Network;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data
{
    public interface IUserAuthenticationService :
        IUserLoginService,
        IUserRegistrationService,
        IUserDataRoleService
    {
        UserDataWithLoginToken GetByLoginTokenHack(string loginToken);
    }
}