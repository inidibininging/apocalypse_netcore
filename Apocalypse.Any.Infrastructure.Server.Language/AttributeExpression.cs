using System;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class AttributeExpression : TerminalExpression
    {
        public string Name { get; set; }

        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if(machine.SharedContext.Current == LexiconSymbol.Position ||
                    machine.SharedContext.Current == LexiconSymbol.Scale ||
                    machine.SharedContext.Current == LexiconSymbol.Color ||
                    machine.SharedContext.Current == LexiconSymbol.Alpha ||
                    machine.SharedContext.Current == LexiconSymbol.Attribute ||
                    machine.SharedContext.Current == LexiconSymbol.Stats)
                Name = machine.SharedContext.CurrentBuffer;
        }
    }
}
