using System;
using System.Linq;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Common.Delegation;
using States.Core.Infrastructure.Services;
using States.Core.Common;

namespace Apocalypse.Any.GameServer.GameInstance
{
    public class EchseStateMachineBridge<TContext> 
        : IStateMachine<string, IEchseContext>
        where TContext : IEchseContext
    {
        private readonly IStateMachine<string, TContext> _stateMachine;

        public EchseStateMachineBridge(IStateMachine<string, TContext> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Run(string key) => _stateMachine.Run(key);
        

        private Func<Dictionary<string, IState<string, IEchseContext>>> getBridgeDictionary => () => 
                    _stateMachine.GetService.States.ToDictionary(
                        keySelector: key => key, 
                        elementSelector: key => 
                            (IState<string, IEchseContext>)new CommandStateActionDelegate<string, IEchseContext>(
                                (sm) => _stateMachine.GetService.Get(key)));
        public IStateGetService<string, IEchseContext> GetService => 
            new DictionaryGetStateDelegationService<string, IEchseContext>(getBridgeDictionary);
        public IStateSetService<string, IEchseContext> SetService => 
            new DictionarySetStateDelegationService<string, IEchseContext>(getBridgeDictionary, 
            (key, state) => this.SetService.Set(key, state));
        public IStateNewService<string, IEchseContext> NewService => 
            new DictionaryNewStateDelegationService<string, IEchseContext>(
                getBridgeDictionary, 
                Guid.NewGuid().ToString,
                (identifier, newState) => 
                    _stateMachine.NewService.New(identifier, 
                    (IState<string, TContext>)new CommandStateActionDelegate<string, TContext>((sm) =>
                       newState.Handle(this)))
            );

        public string SharedIdentifier => _stateMachine.SharedIdentifier;

        public IEchseContext SharedContext
        {
            get => _stateMachine.SharedContext;
            set => throw new NotSupportedException();
        }

        public IReadOnlyDictionary<string, TimeSpan> TimeLog => _stateMachine.TimeLog;
    }
}