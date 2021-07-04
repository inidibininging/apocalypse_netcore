using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.GameInstance
{
    public class EchseStateBridge<TContext> : IState<string, TContext>
        where TContext : IEchseContext
    {
        private readonly IState<string, TContext> _state;

        EchseStateBridge(IState<string, TContext> state)
        {
            _state = state;
        }
        public void Handle(IStateMachine<string, TContext> machine)
        {
            _state.Handle(machine);
        }
    }
}