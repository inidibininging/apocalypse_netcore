using System;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public abstract class AbstractInterpreterInstruction : IState<string, IGameSectorLayerService>
    {
        protected Interpreter Owner { get;}
        protected int ConcurrentExecutions {get;set;}
        protected AbstractInterpreterInstruction(Interpreter interpreter) {
            Owner = interpreter;
        }
        public virtual void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            ConcurrentExecutions++;
        }
    }
}
