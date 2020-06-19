using States.Core.Infrastructure.Services;

namespace States.Core.Common
{
    public class SavedSharedContextState<TKey, TValue> : IState<TKey, TValue>
    {
        public TValue SharedContext { get; set; }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            SharedContext = machine.SharedContext;
        }
    }
}