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
        public const int UpdateCommandSpecific = 4;
        public const int SyncSectorCommand = 5;
        public const int ExeceuteCommand = 6;
        public const int GetGameStateByLoginToken = 7;
        public const int SendPressReleaseCommand = 8;
        public const int AckCommand = 9;
        public const int OutOfSyncCommand = 10;

        public const int BroadcastCommand = 11;
    }
}