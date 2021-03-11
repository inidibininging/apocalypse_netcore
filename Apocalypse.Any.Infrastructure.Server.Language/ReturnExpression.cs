using System;
using System.Linq;
using System.Text;
using Apocalypse.Any.Domain.Common.Model.Language;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{

    public class ReturnExpression : TerminalExpression
    {
        public VariableExpression Value { get; set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            while(Value == null && 
                (machine.SharedContext.Current == LexiconSymbol.Return ||
                machine.SharedContext.Current == LexiconSymbol.SkipMaterial)) {
                machine.SharedContext.MoveNext();
            }
            while(
                machine.SharedContext.Current == LexiconSymbol.TagIdentifier || 
                machine.SharedContext.Current == LexiconSymbol.TagLetter)
            {
                Value = new TagExpression();
                Value.Handle(machine);
            }

            // Console.WriteLine($"Faction set to {Name}");
            if (Value == null)
                throw new ArgumentNullException($"Syntax Error. No Tag expression found near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
