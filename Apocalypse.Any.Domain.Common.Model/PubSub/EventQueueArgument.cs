using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model.PubSub
{
    public class EventQueueArgument : IIdentifiableModel
    {
        public string Id { get; set; }
        public string EventName { get; set; }
        public string DynamicRelationId { get; set; }    
    }
}
