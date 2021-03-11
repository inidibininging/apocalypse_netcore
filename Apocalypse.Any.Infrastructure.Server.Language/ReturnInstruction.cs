using System;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ReturnInstruction : AbstractInterpreterInstruction<ReturnExpression>
    {
        public ReturnInstruction(Interpreter interpreter, ReturnExpression expression, int functionIndex) : base(interpreter, expression, functionIndex)
        {
        }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            Scope.LastReturnValue = Expression;
            base.Handle(machine);
        }
    }
}
