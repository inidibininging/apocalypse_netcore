﻿namespace Apocalypse.Any.Domain.Common.Network
{
    /// <summary>
    /// Collection of all server command constants
    /// </summary>
    public static class NetworkCommandConstants
    {
        public const string ErrorCommand = "e";
        public const string InitializeCommand = "i";
        public const string LoginCommand = "l";
        public const string UpdateCommand = "u";
        public const string ReceiveDataLayerCommand = "w";
        public const string ReceiveMessagesCommand = "m";
        public const string SuggestCommand = "s";
        public const string UpdateCommandDelta = "d";
        public const string ReceiveWorkCommand = "w";
        public const string ExeceuteCommand = "x";
    }
}