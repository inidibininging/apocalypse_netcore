using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces
{
    public interface ISubscribeable<TopicType, ContentType>
    {
        void Subscribe(TopicType topic);
        void Unsubscribe(TopicType topic);
    }
}
