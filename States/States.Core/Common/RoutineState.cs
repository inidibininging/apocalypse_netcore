using States.Core.Infrastructure.Services;
using System.Collections.Generic;

namespace States.Core.Common
{
    public class RoutineState<TKey, TValue> : IState<TKey, TValue>
    {
        public IEnumerable<TKey> Operations { get; set; }

        public void Handle(IStateMachine<TKey, TValue> machine)
        {
            foreach (var op in Operations){                
                machine.Run(op);
            }
        }
    }
}