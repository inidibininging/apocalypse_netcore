using System;
using System.Collections.Generic;
using System.Linq;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class ModifyAttributeExpression : AbstractLanguageExpression
    {
        public VariableExpression Identifier { get; set; }
        public AttributeExpression Section { get; set; }
        public AttributeExpression Attribute { get; set; }
        public SignConverterExpression SignConverter { get; set; }
        public NumberExpression Number { get; set; }
        public List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>() {
                LexiconSymbol.Entity,
                LexiconSymbol.EntityIdentifier,
                LexiconSymbol.EntityLetter,
                LexiconSymbol.FactionIdentifier,
                LexiconSymbol.FactionLetter,
                LexiconSymbol.Attribute,
                LexiconSymbol.Stats,
                LexiconSymbol.NegativeSign,
                LexiconSymbol.PositiveSign,
                LexiconSymbol.Number,
                LexiconSymbol.Position,
                LexiconSymbol.Scale,
                LexiconSymbol.Color,
                LexiconSymbol.Rotation,
                LexiconSymbol.Alpha                
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            // Gate
            if (machine.SharedContext.Current != LexiconSymbol.Modify)
                return;

            while (Identifier == null ||
                  Section == null || 
                  Attribute == null ||
                  SignConverter == null ||
                  Number == null)
            {

                if (!machine.SharedContext.MoveNext())
                    break;
                if (!ValidLexemes.Contains(machine.SharedContext.Current))
                    continue;

                // Console.WriteLine(machine.SharedContext.CurrentBuffer);
                if (machine.SharedContext.Current == LexiconSymbol.EntityIdentifier)
                {
                    Console.WriteLine($"adding {nameof(EntityExpression)}");
                    Identifier = new EntityExpression();
                    Identifier.Handle(machine);
                }
                if (machine.SharedContext.Current == LexiconSymbol.FactionIdentifier)
                {
                    Console.WriteLine($"adding {nameof(FactionExpression)}");
                    Identifier = new FactionExpression();
                    Identifier.Handle(machine);
                }
                if (machine.SharedContext.Current == LexiconSymbol.Attribute)
                {
                    Console.WriteLine($"adding {nameof(AttributeExpression)}");
                    Attribute = new AttributeExpression();
                    Attribute.Handle(machine);
                }
                if (machine.SharedContext.Current == LexiconSymbol.Position ||
                    machine.SharedContext.Current == LexiconSymbol.Scale ||
                    machine.SharedContext.Current == LexiconSymbol.Color ||
                    machine.SharedContext.Current == LexiconSymbol.Alpha ||
                    machine.SharedContext.Current == LexiconSymbol.Stats)
                {
                    Console.WriteLine($"adding section {nameof(AttributeExpression)}");
                    Section = new AttributeExpression();
                    Section.Handle(machine);
                }
                if (machine.SharedContext.Current == LexiconSymbol.PositiveSign ||
                   machine.SharedContext.Current == LexiconSymbol.NegativeSign)
                {
                    Console.WriteLine($"adding {nameof(SignConverterExpression)}");
                    SignConverter = new SignConverterExpression();
                    SignConverter.Handle(machine);
                }
                if (machine.SharedContext.Current == LexiconSymbol.Number)
                {
                    Console.WriteLine($"adding {nameof(NumberExpression)}");
                    Number = new NumberExpression();
                    Number.Handle(machine);
                }
            }
        }

    }
}
