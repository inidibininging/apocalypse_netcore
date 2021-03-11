using System;
using System.Linq;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Language;
using States.Core.Infrastructure.Services;

public class RefExpression : VariableExpression
{
    public override void Handle(IStateMachine<string, Tokenizer> machine)
    {
                    // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var entityName = new StringBuilder();
            while(machine.SharedContext.Current == LexiconSymbol.RefIdentifier || 
                machine.SharedContext.Current == LexiconSymbol.RefLetter)
            {
                if(machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());
                // Console.WriteLine("ok faction");
                // Console.WriteLine(machine.SharedContext.CurrentBuffer);
                if(!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("",entityName.ToString().Skip(1));
            // Console.WriteLine($"Faction set to {Name}");
            if (string.IsNullOrWhiteSpace(Name))
                throw new ArgumentNullException($"Syntax Error. Cannot process Ref Expression Name near {machine.SharedContext.CurrentBuffer}");
    }
}