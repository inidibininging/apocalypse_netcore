using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using MsgPack;
using MsgPack.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Serializer.MsgPackAdapter
{
    public class MsgPackSerializerAdapter : ISerializationAdapter
    {
        public T DeserializeObject<T>(string content)
        {
            var serializer = MessagePackSerializer.Get<T>();
            T subject;
            using (var ms = new MemoryStream(Convert.FromBase64String(content)))
            {
                ms.Position = 0;
                subject = serializer.Unpack(ms);
            }
            return subject;
        }

        public object DeserializeObject(string content, Type type)
        {
            var serializer = MessagePackSerializer.Get(type);
            object subject;
            using (var ms = new MemoryStream(Convert.FromBase64String(content)))
            {
                ms.Position = 0;
                subject = serializer.Unpack(ms);
            }
            return subject;
        }

        public string SerializeObject<T>(T instance)
        {
            var serializer = MessagePackSerializer.Get<T>();
            var finalString = new StringBuilder();
            using (var ms = new MemoryStream())
            {
                serializer.Pack(ms, instance);
                ms.Position = 0;
                finalString.Append(Convert.ToBase64String(ms.ToArray()));
            }
            return finalString.ToString();
        }

        public string SerializeObject(object instance, Type type)
        {
            var serializer = MessagePackSerializer.Get(type);
            var finalString = new StringBuilder();
            using (var ms = new MemoryStream())
            {
                serializer.Pack(ms, instance);
                ms.Position = 0;
                finalString.Append(Convert.ToBase64String(ms.ToArray()));
            }
            return finalString.ToString();
        }
    }
}
