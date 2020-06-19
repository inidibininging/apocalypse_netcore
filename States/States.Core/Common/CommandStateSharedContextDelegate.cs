using States.Core.Infrastructure.Services;
using System;

namespace States.Core.Common
{
    public class CommandStateSharedContextDelegate<TKey, TValue> : IState<TKey, TValue>
    {
        private readonly Func<IStateMachine<TKey, TValue>, TValue> Func;

        public CommandStateSharedContextDelegate(Func<IStateMachine<TKey, TValue>, TValue> func)
        {
            Func = func ?? throw new ArgumentNullException(nameof(func));
        }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            machine.SharedContext = Func(machine);
        }
    }
}