using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces
{
    public interface IGameSectorLayerServiceStateMachineFactory<TBuilderConfigurationKey>
    {
        IStateMachine<string, IGameSectorLayerService> BuildStateMachine(TBuilderConfigurationKey builderConfigurationKey);
    }
}