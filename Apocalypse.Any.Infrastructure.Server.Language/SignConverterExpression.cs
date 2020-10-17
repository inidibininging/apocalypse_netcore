using System;
using Apocalypse.Any.Domain.Common.Model.Language;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class SignConverterExpression : TerminalExpression
    {
        public int Polarity { get; set; } = 0;

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if(machine.SharedContext.Current == LexiconSymbol.NegativeSign)
                Polarity = -1;
            if(machine.SharedContext.Current == LexiconSymbol.PositiveSign)
                Polarity = 1;
        }
    }
}
