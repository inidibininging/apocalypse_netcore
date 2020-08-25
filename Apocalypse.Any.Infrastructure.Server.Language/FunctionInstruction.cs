using System;
using System.Collections.Generic;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class FunctionInstruction : AbstractInterpreterInstruction
    {
        public FunctionExpression Function { get; set; }
        public int FunctionIndex { get; set; } = -1;
        public FunctionInstruction(Interpreter interpreter, FunctionExpression function, int functionIndex) : base(interpreter)
        {
            //Console.WriteLine("adding function instruction " + function.Name);
            Function = function;
            FunctionIndex = functionIndex;
            interpreter.Context.NewService.New(Function.Name,this);
        }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine){
            var currentInstructionIndex = FunctionIndex+1;
            while(currentInstructionIndex < Owner.Instructions.Count){
                var currentInstruction = Owner.Instructions[currentInstructionIndex];
                currentInstructionIndex++;
                if(currentInstruction is FunctionInstruction){
                    //Console.WriteLine("function instruction found. aborting function instruction");
                    break;
                }
                currentInstruction.Handle(machine);
                if(currentInstruction is WaitInstruction)
                {
                    //Console.WriteLine("wait instruction found. aborting function instruction");
                    break;
                }
            }
            //Console.WriteLine("function executed");
        }

        public override string ToString()
        {
            return Function.Name;
        }
    }
}
