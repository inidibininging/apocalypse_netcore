using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Bridge
{
    public class BasicCommandStateImpl : IUnionStateCommand<string, IStateMachine<string, int>>
    {
        public bool CanExecute(IStateMachine<string, int> parameters)
        {
            if (parameters == null)
                return false;
            if (parameters.SharedContext == 0)
                return false;
            return true;
        }

        public void Execute(IStateMachine<string, int> parameters)
        {
        }

        public void Handle(IStateMachine<string, IStateMachine<string, int>> machine)
        {
            if (!CanExecute(machine.SharedContext))
                return;
            Execute(machine.SharedContext);
        }
    }
}