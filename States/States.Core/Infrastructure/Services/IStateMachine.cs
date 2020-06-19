using System;
using System.Collections.Generic;

namespace States.Core.Infrastructure.Services
{
    public interface IStateMachine<TIdentifier, TSharedContext>
    {
        IStateGetService<TIdentifier, TSharedContext> GetService { get; }
        IStateSetService<TIdentifier, TSharedContext> SetService { get; }
        IStateNewService<TIdentifier, TSharedContext> NewService { get; }

        TIdentifier SharedIdentifier { get; }
        TSharedContext SharedContext { get; set; }
        
        IReadOnlyDictionary<TIdentifier, TimeSpan> TimeLog { get; }

        void Run(TIdentifier key);
    }
}