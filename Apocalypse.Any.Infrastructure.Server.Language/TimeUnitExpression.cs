using System;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class TimeUnitExpression : TerminalExpression
    {
        public string Name { get; set; }
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
             if(machine.SharedContext.Current == LexiconSymbol.Miliseconds ||
                machine.SharedContext.Current == LexiconSymbol.Seconds ||
                machine.SharedContext.Current == LexiconSymbol.Minutes ||
                machine.SharedContext.Current == LexiconSymbol.Hours)
            {
                Name = machine.SharedContext.CurrentBuffer;
            }
        }
    }
}
