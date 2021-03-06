using System;
using System.Linq;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Language;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class EntityExpression : VariableExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var entityName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.EntityIdentifier || machine.SharedContext.Current == LexiconSymbol.EntityLetter){
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());
                Console.WriteLine("ok entity");
                Console.WriteLine(machine.SharedContext.CurrentBuffer);
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("",entityName.ToString().Skip(1));
            Console.WriteLine($"Identifier set to {Name}");
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException($"Syntax error: ${nameof(Name)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
