using States.Core.Infrastructure.Services;
using System;

namespace States.Core.Common
{
    public class CommandStateStateDelegate<TKey, TValue> : IState<TKey, TValue>
    {
        private Func<IStateMachine<TKey, TValue>, IState<TKey, TValue>> Func;

        public CommandStateStateDelegate(Func<IStateMachine<TKey, TValue>, IState<TKey, TValue>> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            Func(machine).Handle(machine);
        }
    }
}