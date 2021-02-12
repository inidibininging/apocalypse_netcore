namespace Apocalypse.Any.Domain.Common.Model.Network
{
    /// <summary>
    /// A request message with the sector key attached and the login token
    /// This is used by the ReceiveWork command in order to get a part of a game state data layer
    /// </summary>
    public class ReceiveGameStateDataLayerPartRequest
    {
        public int SectorKey { get; set; }
        public string LoginToken { get; set; }
        public string GetProperty { get; set; }
    }
}