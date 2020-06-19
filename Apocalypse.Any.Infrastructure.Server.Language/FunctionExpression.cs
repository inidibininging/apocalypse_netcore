using System;
using System.Linq;
using System.Text;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class FunctionExpression : VariableExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var functionName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.FunctionIdentifier || machine.SharedContext.Current == LexiconSymbol.FunctionLetter){
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    functionName.Append(machine.SharedContext.CurrenBufferRaw.Last());
                // Console.WriteLine("ok function");
                // Console.WriteLine(machine.SharedContext.CurrentBuffer);
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("",functionName.ToString().Skip(1));
            Console.WriteLine($"function set to {Name}");
        }
    }
}