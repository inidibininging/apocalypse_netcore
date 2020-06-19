using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces
{
    public interface IPublisher<TopicType,ContentType>
    {
        void Publish(TopicType topic, ContentType content);
    }
}
