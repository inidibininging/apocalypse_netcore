using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class DestroyerExpression : VariableExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var entityName = new StringBuilder();
            while ( machine.SharedContext.Current == LexiconSymbol.DestroyerLetter || 
                    machine.SharedContext.Current == LexiconSymbol.Destroy)
            {
                if (machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());                
                if (!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("", entityName.ToString());
        }
    }
}
