using System;

namespace Apocalypse.Any.Infrastructure.Common.Services.Serializer.Interfaces
{
    public interface ISerializationAdapter
    {
        string SerializeObject<T>(T instance);
        string SerializeObject(object instance, Type type);
        T DeserializeObject<T>(string content);
        object DeserializeObject(string content, Type type);
    }
}
