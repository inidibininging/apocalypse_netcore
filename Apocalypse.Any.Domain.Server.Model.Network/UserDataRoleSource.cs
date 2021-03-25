using System;

namespace Apocalypse.Any.Domain.Server.Model.Network
{
    [Flags]

    public enum UserDataRoleSource
    {
        Unset,
        SyncServer,
        LocalServer,
        DesktopClient
        // WebClient,
        // WebAdmin
    }
}