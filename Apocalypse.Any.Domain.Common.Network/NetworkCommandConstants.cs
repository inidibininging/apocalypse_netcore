namespace Apocalypse.Any.Domain.Common.Network
{
    /// <summary>
    /// Collection of all server command constants
    /// </summary>
    public static class NetworkCommandConstants
    {
        public const int ErrorCommand = 6;
        public const int InitializeCommand = 0;
        public const int LoginCommand = 1;
        public const int UpdateCommand = 2;
        public const int UpdateCommandDelta = 3;
        public const int ReceiveWorkCommand = 4;
        public const int ExeceuteCommand = 5;
        public const int GetGameStateByLoginToken = 6;
    }
}