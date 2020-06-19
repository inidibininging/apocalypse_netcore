namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class NetworkCommand
    {
        public string CommandName { get; set; }
        public string CommandArgument { get; set; }
        public string Data { get; set; }
    }
}