using System;
using System.Linq;
using System.Text;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ExecuteExpression : VariableExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var functionName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.Execute || machine.SharedContext.Current == LexiconSymbol.FunctionLetter){
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    functionName.Append(machine.SharedContext.CurrenBufferRaw.Last());                
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("",functionName.ToString().Skip(1));
            Console.WriteLine($"function set to {Name}");
        }
    }
}
