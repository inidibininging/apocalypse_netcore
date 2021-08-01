using Apocalypse.Any.Core;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Server.Services.Mechanics.Bridge
{
    /// <summary>
    /// Implements the state and command pattern
    /// </summary>
    /// <typeparam name="TIdentifier"></typeparam>
    /// <typeparam name="TParamSharedContext"></typeparam>
    public interface IUnionStateCommand<TIdentifier, TParamSharedContext> : ICommand<TParamSharedContext>, IState<TIdentifier, TParamSharedContext>
    {
    }
}