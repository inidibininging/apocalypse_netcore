using Apocalypse.Any.Domain.Common.Model.Language;
using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ApplyMechanicExpression : AbstractLanguageExpression
    {
        public VariableExpression Identifier { get; set; }
        public MechanicExpression Mechanic { get; set; }

        private List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>() {
                LexiconSymbol.ApplyMechanic,
                LexiconSymbol.MechanicLetter,
                LexiconSymbol.TagIdentifier,
                LexiconSymbol.TagLetter,
                LexiconSymbol.EntityIdentifier,
                LexiconSymbol.EntityLetter,
                LexiconSymbol.Letter
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if (machine.SharedContext.Current != LexiconSymbol.ApplyMechanic)
                return;
            while (Identifier == null || Mechanic == null)
            {
                if (!machine.SharedContext.MoveNext())
                    break;
                if (!ValidLexemes.Contains(machine.SharedContext.Current))
                    continue;

                if (machine.SharedContext.Current == LexiconSymbol.MechanicLetter)
                {
                    Console.WriteLine($"adding {nameof(CreatorExpression)}");
                    Mechanic = new MechanicExpression();
                    Mechanic.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.TagIdentifier)
                {
                    Console.WriteLine($"adding {nameof(TagExpression)}");
                    Identifier = new TagExpression();
                    Identifier.Handle(machine);
                }

                if (machine.SharedContext.Current == LexiconSymbol.Letter)
                {
                    Console.WriteLine($"adding {nameof(IdentifierExpression)}");
                    Identifier = new IdentifierExpression();
                    Identifier.Handle(machine);
                }
            }
            if (Mechanic == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Mechanic)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
            if (Identifier == null)
                throw new InvalidOperationException($"Syntax error: ${nameof(Identifier)} side is not implemented near {machine.SharedContext.CurrentBuffer}");
        }
    }
}
