namespace States.Core.Infrastructure.Services
{
    public interface IState<TIdentifier, TSharedContext>
    {
        void Handle(IStateMachine<TIdentifier, TSharedContext> machine);
    }
}