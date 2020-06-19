using System;

namespace Apocalypse.Any.Domain.Server.Model.Network
{
    /// <summary>
    /// Witholds information of the possible roles a user can have on a game server
    /// </summary>
    [Flags]
    public enum UserDataRole
    {
        /// <summary>
        /// Can access to a view port by login token
        /// </summary>
        CanViewWorldByLoginToken,

        /// <summary>
        /// Can send remote input commands
        /// </summary>
        CanSendRemoteMovementCommands,

        /// <summary>
        /// Can send state keys to a game server. You use this if you want to manipulate the control flow of the game server
        /// </summary>
        CanSendRemoteStateCommands,

        /// <summary>
        /// Can create factories in a game server
        /// </summary>
        CanSendRemoteFactoryCommands,

        /// <summary>
        /// Can create singular mechanic commands in a game server
        /// </summary>
        CanSendRemoteSingularMechanicCommands,

        /// <summary>
        /// Can create plural mechanic commands in a game server
        /// </summary>
        CanSendRemotePluralMechanicCommands,

        /// <summary>
        /// Can receive work
        /// </summary>
        CanReceiveWork
    }
}