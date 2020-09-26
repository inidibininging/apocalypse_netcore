using System;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ExecuteInstruction : AbstractInterpreterInstruction<ExecuteExpression>
    {
        public ExecuteInstruction(Interpreter interpreter, ExecuteExpression function, int functionIndex) : base(interpreter, functionIndex, function)
        {
         
        }

        public TagVariable GetVariableOfFunction(IStateMachine<string, IGameSectorLayerService> machine, int argumentIndex)
        {
            TagVariable variable;
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
            var fn = machine.GetService.Get(Expression.Name) as FunctionInstruction;
            fn.LastCaller = this;
            machine.Run(Expression.Name);
        }
    }
}
