using System;
using System.Collections.Generic;
using Echse.Domain;
using States.Core.Common.Delegation;
using States.Core.Infrastructure.Services;

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

        public void Run(string key)
        {
            _stateMachine.Run(key);
        }

        public IStateGetService<string, IEchseContext> GetService => _stateMachine.GetService as IStateGetService<string, IEchseContext>;
        public IStateSetService<string, IEchseContext> SetService => _stateMachine.SetService as IStateSetService<string, IEchseContext>;
        public IStateNewService<string, IEchseContext> NewService => _stateMachine.NewService as IStateNewService<string, IEchseContext>;

        public string SharedIdentifier => _stateMachine.SharedIdentifier;

        public IEchseContext SharedContext
        {
            get => _stateMachine.SharedContext;
            set => throw new NotSupportedException();
        }

        public IReadOnlyDictionary<string, TimeSpan> TimeLog => _stateMachine.TimeLog;
    }
}