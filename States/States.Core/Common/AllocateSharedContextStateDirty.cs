using States.Core.Infrastructure.Services;

namespace States.Core.Common
{
    public class AllocateSharedContextStateDirty<TSavedSharedContextState, TKey, TValue> : IState<TKey, TValue>
        where TSavedSharedContextState : SavedSharedContextState<TKey, TValue>, new()
    {
        public TKey NewKey { get; set; }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            NewKey = machine.NewService.New(new TSavedSharedContextState());
        }
    }
}