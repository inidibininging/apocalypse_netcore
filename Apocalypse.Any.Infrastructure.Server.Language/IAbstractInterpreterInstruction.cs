using Apocalypse.Any.Infrastructure.Server.Services.Data.Interfaces;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Language
{
    public interface IAbstractInterpreterInstruction<out TExpr> : IState<string, IGameSectorLayerService>
        where TExpr : class, IAbstractLanguageExpression
    {
        int FunctionIndex { get; }
        FunctionInstruction Scope { get; }
        // int ScopeIndex { get; }
        TExpr Expression { get; }
    }
}