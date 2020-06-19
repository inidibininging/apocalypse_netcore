using States.Core.Infrastructure.Services;

namespace States.Core.Common
{
    public class LoadSharedContext<TKey, TValue> : IState<TKey, TValue>
    {
        public TValue SharedContext { get; set; } = default(TValue);

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            machine.SharedContext = SharedContext;
        }
    }
}