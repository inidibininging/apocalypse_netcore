using Apocalypse.Any.Domain.Common.Model.Language;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class CreatorExpression : VariableExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Console.WriteLine(machine.SharedContext.CurrentBuffer);
            var entityName = new StringBuilder();
            while (machine.SharedContext.Current == LexiconSymbol.CreatorLetter)
            {
                if (machine.SharedContext.CurrenBufferRaw.Count > 0)
                    entityName.Append(machine.SharedContext.CurrenBufferRaw.Last());                
                if (!machine.SharedContext.MoveNext())
                    break;
            }
            Name = string.Join("", entityName.ToString());
            if (string.IsNullOrWhiteSpace(Name))
                throw new InvalidOperationException($"Syntax error: ${nameof(Name)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
