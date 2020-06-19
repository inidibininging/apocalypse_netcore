using States.Core.Infrastructure.Services;

namespace States.Core.Common
{
    /// <summary>
    /// This allocation context is a representation of an allocated state
    /// The object is represented by a key stored in TKey.
    /// The source stream will be the context where the state is taken from.
    ///
    /// The next key of type TKey pulled from the source stream will be treated as the key that represents the object
    /// This can be seen as a variable and the value of the variable
    ///
    /// </summary>
    /// <typeparam name="TSavedSharedContextState">Saved shared state that will be shared within the state machine</typeparam>
    /// <typeparam name="TKey">The key that represents the shared context</typeparam>
    /// <typeparam name="TValue">The shared context.In this case the shared context will be basically of type SavedSharedContextState</typeparam>
    public class AllocateNamedSharedContextStateDirty<TSavedSharedContextState, TKey, TValue> : IState<TKey, TValue>
        where TSavedSharedContextState : SavedSharedContextState<TKey, TValue>, new()
    {
        public TKey NewKey { get; set; }
        public SharedKeyEnumerable<TKey> SourceStream { get; set; }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            var enumerable = SourceStream.Iterate().GetEnumerator();
            enumerable.MoveNext();
            enumerable.MoveNext();
            NewKey = enumerable.Current;

            machine.NewService.New(NewKey, new TSavedSharedContextState());
            enumerable.MoveNext();
            enumerable.Dispose();
            enumerable = null;
        }
    }
}