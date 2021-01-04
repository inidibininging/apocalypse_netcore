namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public interface INetworkCommand<TCommandIdentifier, TCommandArgument, TData>
    {
        
        TCommandIdentifier CommandName { get; set; }
        TCommandArgument CommandArgument { get; set; }
        TData Data { get; set; }
    }
}