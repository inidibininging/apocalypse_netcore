using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces
{
    public interface ISubscribeable<in TTopicType>
    {
        void Subscribe(TTopicType topic);
        void Unsubscribe(TTopicType topic);
    }
}
