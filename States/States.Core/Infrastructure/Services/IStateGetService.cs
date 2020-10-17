using System.Collections;
using System.Collections.Generic;

namespace States.Core.Infrastructure.Services
{
    public interface IStateGetService<TIdentifier, TSharedContext>
    {
        IState<TIdentifier, TSharedContext> Get(TIdentifier identifier);
        bool HasState(TIdentifier identifier);
        
    }
}