using Apocalypse.Any.Domain.Common.Model.Network;
using Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces;

namespace Apocalypse.Any.Infrastructure.Server.States.Interfaces
{
    public interface INetworkCommandConnectionToGameStateTranslatorService
    {
        INetworkCommandConnectionToGameStateTranslator GetTranslator(NetworkCommandConnection networkCommandConnection);
    }
}