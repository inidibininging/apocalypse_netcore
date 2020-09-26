using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class FunctionInstruction : AbstractInterpreterInstruction<FunctionExpression>
    {
        public ExecuteInstruction LastCaller { get; set; }
        
        public FunctionInstruction(Interpreter interpreter, FunctionExpression function, int functionIndex) : base(interpreter, functionIndex, function)
        {
            Console.WriteLine("adding function instruction " + function.Name);
            interpreter.Context.NewService.New(Expression.Name,this);
        }

        public int GetVariableIndex(string identifierName)
        {
            //look up for the function index in the function                
            return
                Expression
                    .Arguments
                    .Arguments.IndexOf(
                        Expression
                        .Arguments
                        .Arguments
                        .FirstOrDefault(arg => arg.Name == identifierName));
        }
        
        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine) {
            var currentInstructionIndex = FunctionIndex+1;
            while(currentInstructionIndex < Owner.Instructions.Count) {
                var currentInstruction = Owner.Instructions[currentInstructionIndex];
                currentInstructionIndex++;

                if(currentInstruction is FunctionInstruction){
                    Console.WriteLine("function instruction found. aborting function instruction");
                    break;
                }
                currentInstruction.Handle(machine);
                if(currentInstruction is WaitInstruction)
                {
                    Console.WriteLine("wait instruction found. aborting function instruction");
                    break;
                }
            }
            Console.WriteLine($"function {Expression.Name} executed");
        }


        
        public override string ToString()
        {
            return Expression.Name;
        }
    }
}
