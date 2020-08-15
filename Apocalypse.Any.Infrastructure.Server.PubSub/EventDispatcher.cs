using Apocalypse.Any.Core;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.PubSub
{
    /// <summary>
    /// Dispatches events from EventQueueQuery and passes them to the GetListenersQuery
    /// </summary>
    public class EventDispatcher : IEventDispatcher
    {
        private Func<IEnumerable<EventQueue>> EventQueueQuery { get; }
        private Func<IEnumerable<INotifiable<string, EventQueueArgument>>> GetListenersQuery { get; }

        public EventDispatcher(Func<IEnumerable<EventQueue>> eventQueueQuery,
                               Func<IEnumerable<INotifiable<string, EventQueueArgument>>> getListenersQuery)
        {
            EventQueueQuery = eventQueueQuery ?? throw new ArgumentNullException(nameof(eventQueueQuery));
            GetListenersQuery = getListenersQuery ?? throw new ArgumentNullException(nameof(getListenersQuery));
        }

        public void DispatchEvents(GameTime gameTime)
        {
            foreach (var eventQueueGroup in EventQueueQuery().GroupBy(q => q.DisplayName))
            {
                foreach (var eventQueue in eventQueueGroup)
                {
                    var events = eventQueue.DataAsEnumerable<EventQueueArgument>().ToList();

                    foreach (var eventQueueArgument in events)
                    {
                        Console.WriteLine(eventQueueArgument.EventName + " fired");
                        foreach (var listener in GetListenersQuery().Where(l => l != null))
                        {                            
                            listener.Notify(eventQueueGroup.Key, eventQueueArgument);
                        }
                    }
                    eventQueue.Remove<EventQueueArgument>(ev => events.Any(e => e.Id == ev.Id));
                }
            }
        }
    }
}
