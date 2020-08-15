using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.PubSub
{
    /// <summary>
    /// A model for relating an event with any relation
    /// </summary>
    public class EventQueueArgument : IIdentifiableModel
    {
        public string Id { get; set; }
        public string EventName { get; set; }
        public string ReferenceId { get; set; }
        public Type ReferenceType { get; set; }
    }
}
