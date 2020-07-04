using Apocalypse.Any.Domain.Server.Configuration.Model;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.States.Sector.Storage
{
    public interface IGameSectorLayerServiceFactory
    {
        IStateMachine<string, IGameSectorLayerService> BuildStateMachine(IGameSectorData gameServerConfiguration);
    }
}