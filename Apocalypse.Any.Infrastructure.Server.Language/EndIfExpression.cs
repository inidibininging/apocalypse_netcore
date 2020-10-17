using States.Core.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class EndIfExpression : AbstractLanguageExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            if(machine.SharedContext.Current != Domain.Common.Model.Language.LexiconSymbol.EndIf)
                throw new InvalidOperationException($"Syntax Error: EndIf not found in {machine.SharedContext.CurrentBuffer}");
        }
    }
}
