using States.Core.Infrastructure.Services;

namespace States.Core.Common
{
    public class LoadStateDirty<TSavedSharedContextState, TKey, TValue> : IState<TKey, TValue>
        where TSavedSharedContextState : SavedSharedContextState<TKey, TValue>
    {
        public TKey Source { get; set; } = default(TKey);
        public SharedKeyEnumerable<TKey> SourceStream { get; set; }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            var enumerable = SourceStream.Iterate().GetEnumerator();
            enumerable.MoveNext();
            enumerable.MoveNext();
            Source = enumerable.Current;
            enumerable.Dispose();
            enumerable = null;
            var savedSharedContext = machine.GetService.Get(Source) as TSavedSharedContextState;
            machine.SharedContext = savedSharedContext.SharedContext;
        }
    }
}