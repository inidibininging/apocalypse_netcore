using System;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public class SkipExpression : TerminalExpression
    {
        public override void Handle(IStateMachine<string, Tokenizer> machine)
        {
            throw new NotImplementedException();
        }
    }
}
