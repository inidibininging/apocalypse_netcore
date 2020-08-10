using Apocalypse.Any.Core.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Domain.Common.Model
{
    public class PlayerDialogEvent<TEventIdentifier, TEventArg> : IIdentifiableModel
        where TEventArg : IIdentifiableModel
    {
        public string Id { get; set; }
        public string DialogNodeId { get; set; }
        public TEventIdentifier Event { get; set; }
        public TEventArg EventArgument { get; set; }
    }
}
