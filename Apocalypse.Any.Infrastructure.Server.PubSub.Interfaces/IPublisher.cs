using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces
{
    public interface IPublisher<in TTopicType, in TContentType>
    {
        void Publish(TTopicType topic, TContentType content);
    }
}
