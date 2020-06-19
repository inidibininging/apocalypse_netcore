﻿using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    /// <summary>
    /// Used for creating stuff and appending a unique id to 
    /// </summary>
    public class DestroyExpression : AbstractLanguageExpression
    {
        public VariableExpression Identifier { get; set; }
        //public DestroyerExpression Destroyer { get; set; }

        public List<LexiconSymbol> ValidLexemes { get; set; } = new List<LexiconSymbol>() {
                LexiconSymbol.Destroy,
                LexiconSymbol.Destroyer,
                //LexiconSymbol.DestroyerLetter,
                LexiconSymbol.FactionIdentifier,
                LexiconSymbol.FactionLetter,
                LexiconSymbol.EntityIdentifier,
                LexiconSymbol.EntityLetter
        };

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if (machine.SharedContext.Current != LexiconSymbol.Destroy)
                return;
            while (Identifier == null)
            {

                if (!machine.SharedContext.MoveNext())
                    break;
                if (!ValidLexemes.Contains(machine.SharedContext.Current))
                    continue;

                //if (machine.SharedContext.Current == LexiconSymbol.DestroyerLetter)
                //{
                //    Console.WriteLine($"adding {nameof(DestroyerExpression)}");
                //    Identifier = new DestroyerExpression();
                //    Identifier.Handle(machine);
                //}
                if (machine.SharedContext.Current == LexiconSymbol.FactionIdentifier)
                {
                    Console.WriteLine($"adding {nameof(FactionExpression)}");
                    Identifier = new FactionExpression();
                    Identifier.Handle(machine);
                }
            }
        }
    }
}
