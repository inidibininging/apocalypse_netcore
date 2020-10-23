using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ExecuteInstruction : AbstractInterpreterInstruction<ExecuteExpression>
    {
        public ExecuteInstruction(Interpreter interpreter, ExecuteExpression function, int functionIndex) 
            : base(interpreter, function, functionIndex)
        {

        }

        public TagVariable GetVariableOfFunction(IStateMachine<string, IGameSectorLayerService> machine, int argumentIndex)
        {
            //look up for the variable inside the execution instruction
            var callerArgument =
                Expression
                    .Arguments
                    .Arguments
                    .ElementAt(argumentIndex);

            //look up for the tag variable 
            return
                machine
                    .SharedContext
                    .DataLayer
                    .GetLayersByType<TagVariable>()
                    .FirstOrDefault()?
                    .DataAsEnumerable<TagVariable>()
                    .FirstOrDefault(t => t.Name == callerArgument.Name &&
                                         t.Scope == Scope
                                             .Expression
                                             .Name);
        }
        
        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            var fn = machine.GetService.Get(Expression.Name);
            if(fn == null)
                throw new KeyNotFoundException(Expression.Name + " not found");
            if((fn as FunctionInstruction) != null)
                (fn as FunctionInstruction).LastCaller = this;
            machine.Run(Expression.Name);
        }
    }
}
