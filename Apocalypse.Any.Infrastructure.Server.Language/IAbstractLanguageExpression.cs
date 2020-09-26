using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public interface IAbstractLanguageExpression : IState<string, Tokenizer>
    {
        
    }
}