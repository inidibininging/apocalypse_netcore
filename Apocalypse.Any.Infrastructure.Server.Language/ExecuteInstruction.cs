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
            //return null if argument index is below 0
            if (argumentIndex < 0)
                return null;
            
            //look up for the variable inside the execution instruction
            var callerArgument =
                Expression
                    .Arguments
                    .Arguments[argumentIndex];

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

        private TagVariable GetVariable(string name, IStateMachine<string, IGameSectorLayerService> machine)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .DataLayer
                .GetLayersByType<TagVariable>()
                .FirstOrDefault()
                ?.DataAsEnumerable<TagVariable>()
                .FirstOrDefault(t => t.Name == name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = name;
            while (variable == null)
            {
                var argumentIndex = lastFn.GetFunctionArgumentIndex(identifierName);
                
                if (argumentIndex < 0)
                    return null;
                    // throw new ArgumentNullException(nameof(identifierName),
                    //                                             $"Tag with name {identifierName} not found inside {Scope.Expression.Name}");

                variable = lastFn
                    .LastCaller
                    .GetVariableOfFunction(machine, argumentIndex);
                if (variable != null)
                    continue;
                identifierName = lastFn
                    .LastCaller
                    .Expression
                    .Arguments
                    .Arguments
                    [argumentIndex]
                    .Name;
                lastFn = lastFn.LastCaller.Scope;

            }
            if (variable.DataTypeSymbol != LexiconSymbol.TagDataType &&
                variable.DataTypeSymbol != LexiconSymbol.RefDataType)
                throw new InvalidOperationException($"Syntax error. Cannot execute instruction. Data type of variable is not a tag.");
            // Console.WriteLine($"Variable:{variable.Name} Current Value:{variable.Value}");
            // Console.WriteLine(System.Environment.NewLine);
            return variable;
        }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            var fn = GetFunctionName(machine);

            if (string.IsNullOrWhiteSpace(fn))
                throw new KeyNotFoundException(fn + " not found");
            
            var fnAsFunction = machine.GetService.Get(fn);
            if (fnAsFunction is FunctionInstruction)
                (fnAsFunction as FunctionInstruction).LastCaller = this;
            machine.Run(fn);
        }

        private string GetFunctionName(IStateMachine<string, IGameSectorLayerService> machine)
        {
            try
            {
                TagVariable fnAsVariable = GetVariable(Expression.Name, machine);
                if(fnAsVariable != null && fnAsVariable.DataTypeSymbol == LexiconSymbol.RefDataType)
                {
                    //check first if the function given is an identifier of type ref because ref can be a function ref
                    return fnAsVariable.Value;
                }
            }
            catch(ArgumentNullException ex)
            {
                Console.WriteLine(ex.Message);
            }
            catch(InvalidOperationException ex)
            {
                throw;
            }
            return Expression.Name;
        }
    }
}
