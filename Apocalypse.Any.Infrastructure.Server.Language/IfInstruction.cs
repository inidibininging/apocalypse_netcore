using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class IfInstruction : AbstractInterpreterInstruction<IfExpression>
    {
        public bool LastEvaluation { get; set; } = false;
        public int EndIfIndex { get; set; } = -1;
        public IfInstruction(Interpreter interpreter, IfExpression expression, int functionIndex) 
            : base(interpreter, expression, functionIndex)
        {
            
        }
        private TagVariable GetVariable(IStateMachine<string, IGameSectorLayerService> machine)
        {
            //get the variable out of the function scope 
            var variable = machine
                .SharedContext
                .DataLayer
                .GetLayersByType<TagVariable>()
                .FirstOrDefault()
                ?.DataAsEnumerable<TagVariable>()
                .FirstOrDefault(t => t.Name == Expression.Left.Name &&
                                     t.Scope == Scope?.Expression.Name);

            var lastFn = Scope;
            var identifierName = Expression.Left.Name;
            while (variable == null)
            {
                var argumentIndex = lastFn.GetFunctionArgumentIndex(identifierName);
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
                    .ElementAt(argumentIndex)
                    .Name;
                lastFn = lastFn.LastCaller.Scope;
                
            }
            if (variable.DataTypeSymbol != LexiconSymbol.TagDataType)
                throw new InvalidOperationException($"Syntax error. Cannot execute a modify instruction. Data type of variable is not a tag.");
            Console.WriteLine($"Variable:{variable.Name} Current Value:{variable.Value}");
            Console.WriteLine(System.Environment.NewLine);
            return variable;
        }
        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            LastEvaluation = Expression.Comparison.ComparisonSymbol == LexiconSymbol.Equal &&
                GetVariable(machine)?.Value == Expression.Right.Name;

            EndIfIndex = FunctionIndex;
            var endIfInstructionFound = false;
            var currentInstructionIndex = EndIfIndex;
            while (!(endIfInstructionFound = Owner.Instructions[EndIfIndex += 1] is EndIfInstruction) &&
                EndIfIndex <= Owner.Instructions.Count)
            {
                if (!LastEvaluation)
                    continue;
                var currentInstruction = Owner.Instructions[EndIfIndex];
                if (currentInstruction is EndIfInstruction)
                {
                    Console.WriteLine("EndIf instruction found");
                    break;
                }

                Console.WriteLine(currentInstruction);
                if (currentInstruction is FunctionInstruction)
                {
                    Console.WriteLine("function instruction found. aborting function instruction");
                    break;
                }
                currentInstruction.Handle(machine);

                if (currentInstruction is IfInstruction)
                {
                    /* If there is a nested if, the if instruction hasn't got the EndIfIndex for the current If yet. 
                     * Example:
                     * 
                        If a is .foo                    # true  -> this is current function
                            If b is .bar                # false -> if this instruction is false, it jumps back to this scope here
                                Set Tag b = .blablabla  # this line should be skipped, from HERE (by here I mean the THIS IfInstructon)
                            EndIf                       # this is line should not be touched at all
                            Set Tag c = .goo            # this line should NOT BE ignored
                        EndIf                           # this is the EndIfIndex we want
                     */
                    EndIfIndex = (currentInstruction as IfInstruction).EndIfIndex; // Does it need +1 ???
                    break;
                }
                if (currentInstruction is WaitInstruction)
                {
                    Console.WriteLine("wait instruction found. aborting function instruction");
                    break;
                }
            }

            base.Handle(machine);
        }
    }
}
