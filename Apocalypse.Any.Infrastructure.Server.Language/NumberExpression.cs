using System;
using System.Linq;
using System.Text;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class NumberExpression : TerminalExpression
    {
        public int? NumberValue { get; set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            var entityName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.Number){
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            NumberValue = int.Parse(entityName.ToString());
            if(NumberValue.HasValue)
                Console.WriteLine($"Parsed Number: {NumberValue.Value}");
            else
                Console.WriteLine("no valid found");
        }
    }
}
