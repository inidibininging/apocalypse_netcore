namespace Apocalypse.Any.Client.States
{
    /// <summary>
    ///     Resource strings containing all the general states as constant strings
    ///     This class SHOULD be converted into a real serializable dictionary
    /// </summary>
    public static class ClientGameScreenBook
    {
        public const string Init = nameof(Init);

        public const string BuildClientSideState = nameof(BuildClientSideState);

        public const string BuildUser = nameof(BuildUser);
        public const string BuildGameSheetFrames = nameof(BuildGameSheetFrames);
        public const string BuildInputService = nameof(BuildInputService);
        public const string BuildDefaultInputService = nameof(BuildDefaultInputService);
        public const string BuildClient = nameof(BuildClient);

        public const string Unload = nameof(Unload);
        public const string Login = nameof(Login);
        public const string Logout = nameof(Logout);
        public const string CheckLoginSent = nameof(CheckLoginSent);
        public const string Connect = nameof(Connect);

        public const string Update = nameof(Update);
        public const string UpdateInput = nameof(UpdateInput);

        public const string BuildCursor = nameof(BuildCursor);
        public const string UpdateCursor = nameof(UpdateCursor);

        public const string UpdateCamera = nameof(UpdateCamera);
        public const string UpdateScreen = nameof(UpdateScreen);
        public const string UpdateMetadataState = nameof(UpdateMetadataState);
        public const string UpdateImages = nameof(UpdateImages);

        public const string CreateFetchDataIfNotExists = nameof(CreateFetchDataIfNotExists);
        public const string FetchData = nameof(FetchData);
        public const string SendGameStateUpdateData = nameof(SendGameStateUpdateData);
        public const string ReadServerDataFromConsole = nameof(ReadServerDataFromConsole);
        public const string Load = nameof(Load);
        public const string FillWithDefaultServerDataState = nameof(FillWithDefaultServerDataState);

        #region UI

        public const string BuildInventoryWindow = nameof(BuildInventoryWindow);
        public const string UpdateInventoryWindow = nameof(UpdateInventoryWindow);
        public const string UpdateInventoryImages = nameof(UpdateInventoryImages);

        public const string BuildChatWindow = nameof(BuildChatWindow);
        public const string UpdateChatWindow = nameof(UpdateChatWindow);

        public const string BuildInfoWindow = nameof(BuildInfoWindow);
        public const string UpdateInfoWindow = nameof(UpdateInfoWindow);

        public const string BuildCharacterWindow = nameof(BuildCharacterWindow);
        public const string UpdateCharacterWindow = nameof(UpdateCharacterWindow);

        public const string BuildTradeWindow = nameof(BuildTradeWindow);
        public const string UpdateTradeWindow = nameof(UpdateTradeWindow);

        public const string UpdateUI = nameof(UpdateUI);

        #endregion UI
    }
}