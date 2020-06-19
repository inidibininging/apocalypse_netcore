using States.Core.Infrastructure.Services;

namespace States.Core.Common
{
    public class CopyStateDirty<TSavedSharedContextState, TKey, TValue> : IState<TKey, TValue>
        where TSavedSharedContextState : SavedSharedContextState<TKey, TValue>
    {
        public TKey Source { get; set; }
        public TKey Destination { get; set; }
        public SharedKeyEnumerable<TKey> SourceStream { get; set; }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            var currentState = machine.SharedContext;

            var enumerable = SourceStream.Iterate().GetEnumerator();
            enumerable.MoveNext();
            enumerable.MoveNext();
            Source = enumerable.Current;

            enumerable.MoveNext();
            Destination = enumerable.Current;
            enumerable.Dispose();
            enumerable = null;

            var source = machine.GetService.Get(Source) as TSavedSharedContextState;
            var destination = machine.GetService.Get(Destination) as TSavedSharedContextState;
            destination.SharedContext = source.SharedContext;
        }
    }
}