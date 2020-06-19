using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces
{
    public interface ISubscriber<TopicType>
    {
        void Subscribe(TopicType topic);
        void Unsubscribe(TopicType topic);
    }
}
