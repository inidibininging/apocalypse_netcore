namespace States.Core.Infrastructure.Services
{
    /// <summary>
    /// Creates a new state of context
    /// </summary>
    /// <typeparam name="TIdentifier">The identifier type</typeparam>
    /// <typeparam name="TSharedContext">The context type</typeparam>
    public interface IStateNewService<TIdentifier, TSharedContext>
    {
        TIdentifier New(IState<TIdentifier, TSharedContext> state);

        TIdentifier New(TIdentifier identifier, IState<TIdentifier, TSharedContext> state);
    }
}