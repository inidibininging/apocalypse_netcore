using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Server.PubSub.Interfaces
{
    public interface INotifiable<TopicType, ContentType>
    {
        void Notify(TopicType topic, ContentType content);
    }
}
