using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Sector.Services
{
    public class RoutingService<TSource, TDestination>
        : IState<string, IGameSectorsOwner>
    {
        public void Handle(IStateMachine<string, IGameSectorsOwner> machine)
        {
        }
    }
}