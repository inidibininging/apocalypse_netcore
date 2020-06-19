using Apocalypse.Any.Core;
using States.Core.Infrastructure.Services;

namespace Apocalypse.Any.Infrastructure.Common.Services.Network.Interfaces.Mechanics.Bridge
{
    /// <summary>
    /// Wraps a command as a shared context so that it can be interpreted and executed as a command
    /// </summary>
    /// <typeparam name="TIdentifier"></typeparam>
    /// <typeparam name="TSharedContext"></typeparam>
    public interface ICommandState<TIdentifier, TSharedContext> : IState<TIdentifier, ICommand<TSharedContext>>
    {
    }
}