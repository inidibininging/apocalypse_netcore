using System;
using System.Linq;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Common.Delegation;
using States.Core.Infrastructure.Services;
using States.Core.Common;
using Echse.Language;

namespace Apocalypse.Any.GameServer.GameInstance
{
    public class EchseStateMachineBridge<TContext> 
        : StateMachine<string, TContext>, 
        IStateMachine<string, IEchseContext>
        
        where TContext : IEchseContext
    {
        private readonly IStateMachine<string, TContext> _stateMachine;

        public EchseStateMachineBridge(IStateMachine<string, TContext> stateMachine) 
            : base(getService: stateMachine.GetService,
                   setService: stateMachine.SetService,
                   newService: stateMachine.NewService)
        {
            _stateMachine = stateMachine;            
        }

        public void Run(string key) => _stateMachine.Run(key);
        

        private Func<Dictionary<string, IState<string, IEchseContext>>> getBridgeDictionary => () => 
                    _stateMachine.GetService.States.ToDictionary(
                        keySelector: key => key, 
                        elementSelector: key => 
                            (IState<string, IEchseContext>)new EchseLanguagePluginWrapperState<string, IEchseContext, TContext>(
                                _stateMachine.GetService.Get(key),
                                this));
        IStateGetService<string, IEchseContext> IStateMachine<string, IEchseContext>.GetService => 
            new DictionaryGetStateDelegationService<string, IEchseContext>(getBridgeDictionary);
        IStateSetService<string, IEchseContext> IStateMachine<string, IEchseContext>.SetService => 
            new DictionarySetStateDelegationService<string, IEchseContext>(getBridgeDictionary, 
            (key, state) => ((IStateMachine<string, IEchseContext>)this).SetService.Set(key, state));
        IStateNewService<string, IEchseContext> IStateMachine<string, IEchseContext>.NewService => 
            new DictionaryNewStateDelegationService<string, IEchseContext>(
                getBridgeDictionary, 
                Guid.NewGuid().ToString,
                (identifier, newState) => {                    
                    _stateMachine.NewService.New(identifier, 
                    new EchseLanguagePluginWrapperState<string, TContext, IEchseContext>(
                        newState,
                        this
                    ));
                }
                    
            );

        string IStateMachine<string, IEchseContext>.SharedIdentifier => _stateMachine.SharedIdentifier;

        IEchseContext IStateMachine<string, IEchseContext>.SharedContext
        {
            get => _stateMachine.SharedContext;
            set => throw new NotSupportedException();
        }

        IReadOnlyDictionary<string, TimeSpan> IStateMachine<string, IEchseContext>.TimeLog => _stateMachine.TimeLog;

        
    }
}