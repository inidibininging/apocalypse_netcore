using Apocalypse.Any.Core.Model;
using Echse.Net.Domain;

namespace Apocalypse.Any.Domain.Common.Model.Network
{
    public class IdentifiableNetworkCommand : NetworkCommand, IIdentifiableModel
    {
        public string Id { get; set; }
    }
}