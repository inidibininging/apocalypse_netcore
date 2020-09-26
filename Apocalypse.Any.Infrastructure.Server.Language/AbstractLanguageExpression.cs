using System;
using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public abstract class AbstractLanguageExpression : IAbstractLanguageExpression
    {
        public abstract void Handle(IStateMachine<string, Tokenizer> machine);
    }
}
