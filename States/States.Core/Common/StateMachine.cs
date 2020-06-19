using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.IO;

namespace States.Core.Common
{
    public class StateMachine<TKey, TValue> : IStateMachine<TKey, TValue>
    {
        public StateMachine
            (
                IStateGetService<TKey, TValue> getService,
                IStateSetService<TKey, TValue> setService,
                IStateNewService<TKey, TValue> newService
            )
        {
            GetService = getService;
            SetService = setService;
            NewService = newService;
        }

        public TValue SharedContext { get; set; }

        public IStateGetService<TKey, TValue> GetService { get; private set; }

        public IStateSetService<TKey, TValue> SetService { get; private set; }

        public IStateNewService<TKey, TValue> NewService { get; private set; }
        public TKey SharedIdentifier { get; private set; }


        private Dictionary<TKey,TimeSpan> timeLog;

        public IReadOnlyDictionary<TKey, TimeSpan> TimeLog
        {
            get { return timeLog; }            
        }
        public void Run(TKey state)
        {            
            if (timeLog == null)
                timeLog = new Dictionary<TKey, TimeSpan>();
            var start = DateTime.Now;
            SharedIdentifier = state;
            GetService.Get(state).Handle(this);
            timeLog[state] = DateTime.Now - start;
        }
        public virtual TStateMachine Run<TStateMachine>(
            TStateMachine machine,
            SharedKeyEnumerable<TKey> stream)
            where TStateMachine : StateMachine<TKey, TValue>
        {
            var enumerator = stream.Iterate().GetEnumerator();
            if (timeLog == null)
                timeLog = new Dictionary<TKey, TimeSpan>();
            while (enumerator.MoveNext())
            {                                
                var start = DateTime.Now;
                machine.Run(enumerator.Current);
                timeLog[enumerator.Current] = DateTime.Now - start;
            }

            return machine;
        }


    }
}