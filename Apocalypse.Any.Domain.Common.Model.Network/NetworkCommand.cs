namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class NetworkCommand : INetworkCommand<byte, string, string>
    {
        public byte CommandName { get; set; }
        public string CommandArgument { get; set; }
        public string Data { get; set; }
    }
}