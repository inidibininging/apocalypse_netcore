using Apocalypse.Any.Core.Model;
using Apocalypse.Any.Domain.Common.Model.PubSub;
using Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.GameServer.Domain
{
    public interface IIdentifiableNotifiableModel
        : INotifiable<string, EventQueueArgument>, IIdentifiableModel
    {
    }
}
