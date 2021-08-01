using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Domain.Server.Model;
using Apocalypse.Any.Domain.Server.Model.Interfaces;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Apocalypse.Any.Infrastructure.Server.Services.Data;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Apocalypse.Any.Infrastructure.Server.PubSub
{
    public class EventQueue : GenericInMemoryDataLayer<EventQueueArgument>, IEventQueue
    {
        public EventQueue(string name) : base(name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(nameof(name));
        }
    }
}
