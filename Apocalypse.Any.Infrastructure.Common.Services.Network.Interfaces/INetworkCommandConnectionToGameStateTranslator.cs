using Apocalypse.Any.Core.Input;
using Apocalypse.Any.Domain.Common.Model.Network;
using Echse.Net.Domain;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces
{
    /// <summary>
    /// Transforms any NetworkCommandConnection (Serverside NetworkCommand object) into a gamestatedata
    /// This interface should be used by translators on server side (used in game states on server side)
    /// </summary>
    public interface INetworkCommandConnectionToGameStateTranslator : IInputTranslator<NetworkCommandConnection, GameStateData>
    {
    }
}