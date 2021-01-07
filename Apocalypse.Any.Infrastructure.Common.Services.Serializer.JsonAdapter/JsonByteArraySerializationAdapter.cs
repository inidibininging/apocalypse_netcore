using Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apocalypse.Any.Infrastructure.Common.Services.Serializer.JsonAdapter
{
    public class JsonByteArraySerializationAdapter : IByteArraySerializationAdapter
    {
        private JsonSerializerAdapter JsonSerializerAdapter { get; set; } = new JsonSerializerAdapter();

        public T DeserializeObject<T>(byte[] content)
        {
            return JsonSerializerAdapter.DeserializeObject<T>(Encoding.UTF8.GetString(content));
        }

        public object DeserializeObject(byte[] content, Type type)
        {
            return JsonSerializerAdapter.DeserializeObject(Encoding.UTF8.GetString(content), type);
        }

        public byte[] SerializeObject<T>(T instance)
        {
            return Encoding.UTF8.GetBytes(JsonSerializerAdapter.SerializeObject<T>(instance));
        }

        public byte[] SerializeObject(object instance, Type type)
        {
            return Encoding.UTF8.GetBytes(JsonSerializerAdapter.SerializeObject(instance, type));
        }
    }
}
