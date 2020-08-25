using System;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ExecuteInstruction : AbstractInterpreterInstruction
    {
        ExecuteExpression Function { get; }
        private int FunctionIndex { get; }
        public ExecuteInstruction(Interpreter interpreter, ExecuteExpression function, int functionIndex) : base(interpreter)
        {
            Function = function;
            FunctionIndex = functionIndex;
        }

        public override void Handle(IStateMachine<string, IGameSectorLayerService> machine)
        {
            // if(this.ConcurrentExecutions > 0){
            //     Console.WriteLine("One execution only");
            //     return;
            // }

            // base.Handle(machine);

            //Console.ForegroundColor = ConsoleColor.Green;
            //Console.WriteLine($"executing {Function.Name}...");
            //Console.ForegroundColor = ConsoleColor.White;

            machine.Run(Function.Name);

            // this.ConcurrentExecutions--;
        }
    }
}
