using System;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.GameServer.GameInstance
{
    public class EchseStateMachineBridge<TContext> 
        : IStateMachine<string, IEchseContext>
        where TContext : IEchseContext
    {
        private readonly IStateMachine<string, TContext> _stateMachine;

        EchseStateMachineBridge(IStateMachine<string, TContext> stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Run(string key)
        {
            throw new NotImplementedException();
        }

        public IStateGetService<string, IEchseContext> GetService => _stateMachine.GetService as IStateGetService<string, IEchseContext>;
        public IStateSetService<string, IEchseContext> SetService => _stateMachine.SetService;
        public IStateNewService<string, IEchseContext> NewService => _stateMachine.NewService;
        
        public string SharedIdentifier { get; }
        public IEchseContext SharedContext { get; set; }
        public IReadOnlyDictionary<string, TimeSpan> TimeLog { get; }
    }
}