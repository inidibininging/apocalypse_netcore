using Apocalypse.Any.Core.Model;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class IdentifiableNetworkCommand : NetworkCommand, IIdentifiableModel
    {
        public string Id { get; set; }
    }
}