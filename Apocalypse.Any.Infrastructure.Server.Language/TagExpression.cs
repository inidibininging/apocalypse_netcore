using System;
using System.Linq;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Language;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class TagExpression : VariableExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var entityName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.TagIdentifier || 
                machine.SharedContext.Current == LexiconSymbol.TagLetter)
            {
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());
                Console.WriteLine("ok faction");
                Console.WriteLine(machine.SharedContext.CurrentBuffer);
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("",entityName.ToString().Skip(1));
            Console.WriteLine($"Faction set to {Name}");
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentNullException($"Syntax Error. Cannot process Tag Expression Name near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
