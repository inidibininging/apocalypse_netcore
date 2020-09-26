using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    /// <summary>
    /// Used for creating stuff and appending a unique id to 
    /// </summary>
    public class CreateExpression : AbstractLanguageExpression
    {
        public VariableExpression Identifier { get; set; }
        public CreatorExpression Creator { get; set; }

        private List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>() {
                LexiconSymbol.CreatorIdentifier,
                LexiconSymbol.CreatorLetter,
                LexiconSymbol.TagIdentifier,
                LexiconSymbol.TagLetter,
                LexiconSymbol.EntityIdentifier,
                LexiconSymbol.EntityLetter
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if (machine.SharedContext.Current != LexiconSymbol.Create)
                return;
            while (Identifier == null || Creator == null)
            {
                if (!machine.SharedContext.MoveNext())
                    break;
                if (!ValidLexemes.Contains(machine.SharedContext.Current))
                    continue;

                if (machine.SharedContext.Current == LexiconSymbol.CreatorLetter)
                {
                    Console.WriteLine($"adding {nameof(CreatorExpression)}");
                    Creator = new CreatorExpression();
                    Creator.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier)
                {
                    Console.WriteLine($"adding {nameof(FactionExpression)}");
                    Identifier = new FactionExpression();
                    Identifier.Handle(machine);
                }
            }
        }
    }
}
