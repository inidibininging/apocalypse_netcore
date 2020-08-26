using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces
{
    public interface IIdentifiableNotifiableModel
        : INotifiable<string, EventQueueArgument>, IIdentifiableModel
    {

    }
}
