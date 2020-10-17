using States.Core.Infrastructure.Services;
using System;

namespace States.Core.Common
{
    /// <summary>
    /// Command pattern inside a state pattern
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    public class CommandStateActionDelegate<TKey, TValue> : IState<TKey, TValue>
    {
        private Action<IStateMachine<TKey, TValue>> Action { get; }

        public CommandStateActionDelegate(Action<IStateMachine<TKey, TValue>> action)
        {
            Action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Handle(IStateMachine<TKey, TValue> machine) => Action(machine);
    }
}